using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using Autofac;
using Orchard;
using Orchard.Data;
using Proligence.PowerShell.Provider.Console;
using Proligence.PowerShell.Provider.Console.Host;
using Proligence.PowerShell.Provider.Console.UI;
using Proligence.PowerShell.Provider.Internal;
using Proligence.PowerShell.Provider.Vfs;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// Represents a PowerShell user session.
    /// </summary>
    public class PsSession : MarshalByRefObject, IPsSession {
        private readonly ConcurrentQueue<string> _queue;
        private readonly RunspaceConfiguration _configuration;
        private readonly IComponentContext _componentContext;
        private readonly Runspace _runspace;
        private readonly AutoResetEvent _runspaceLock;
        private readonly AutoResetEvent _bufferLock;

        public PsSession(
            ConsoleHost consoleHost,
            RunspaceConfiguration configuration,
            IComponentContext componentContext,
            string connectionId) {
            ConsoleHost = consoleHost;
            ConnectionId = connectionId;

            _queue = new ConcurrentQueue<string>();
            _configuration = configuration;
            _componentContext = componentContext;
            _runspace = RunspaceFactory.CreateRunspace(consoleHost, configuration);
            _runspaceLock = new AutoResetEvent(true);
            _bufferLock = new AutoResetEvent(true);

            Runspace.StateChanged += OnRunspaceStateChanged;
        }

        public event EventHandler<Console.DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Gets the console host which provides input/output to the PowerShell engine.
        /// </summary>
        public ConsoleHost ConsoleHost { get; private set; }

        /// <summary>
        /// Gets the configuration of the PowerShell runspace associated with the session.
        /// </summary>
        public RunspaceConfiguration RunspaceConfiguration {
            get { return _configuration; }
        }

        /// <summary>
        /// Gets the PowerShell runspace associated with the session.
        /// </summary>
        public Runspace Runspace {
            get { return _runspace; }
        }

        /// <summary>
        /// Gets the lock which must be acquired before acessing the session's runspace.
        /// </summary>
        public EventWaitHandle RunspaceLock {
            get { return _runspaceLock; }
        }

        /// <summary>
        /// Gets the current session's runspace absolute path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Delegate used for sending messages up to the user console.
        /// </summary>
        public Action<OutputData> Sender { get; internal set; }

        /// <summary>
        /// Gets the dependency injection container for the Orchard application.
        /// </summary>
        public IComponentContext ComponentContext {
            get { return _componentContext; }
        }

        /// <summary>
        /// Gets the Orchard drive instance for this session.
        /// </summary>
        public OrchardDriveInfo OrchardDrive { get; set; }

        /// <summary>
        /// Gets the session's Orchard VFS instance.
        /// </summary>
        public IPowerShellVfs Vfs {
            get {
                if (OrchardDrive != null) {
                    return OrchardDrive.Vfs;
                }

                return null;
            }
        }

        /// <summary>
        /// Connection identifier for this particular session.
        /// </summary>
        public string ConnectionId { get; private set; }

        public override object InitializeLifetimeService() {
            return null;
        }

        /// <summary>
        /// Initializes the session.
        /// </summary>
        public void Initialize() {
            // NOTE (MD):
            // For some strange reason the initialization scripts from the runtime configuration are not executed
            // while opening the runspace, so we need to execute them manually. #OPS-60
            var runspaceConfiguration = _runspace.RunspaceConfiguration;
            var initScripts = runspaceConfiguration.InitializationScripts.ToArray();
            runspaceConfiguration.InitializationScripts.Reset();

            _runspace.Open();

            foreach (ScriptConfigurationEntry entry in initScripts) {
                using (var pipeline = _runspace.CreatePipeline(entry.Definition)) {
                    pipeline.Commands.AddScript(entry.Definition);
                    pipeline.Invoke();
                }
            }
        }

        /// <summary>
        /// Reads line of string from input buffer. Nonblocking.
        /// </summary>
        public string ReadInputBuffer() {
            string result;
            if (_queue.TryDequeue(out result)) {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Writes a line to the input buffer.
        /// </summary>
        public void WriteInputBuffer(string line) {
            _bufferLock.WaitOne();
            _queue.Enqueue(line);
            OnDataReceived(new Console.DataReceivedEventArgs());
        }

        /// <summary>
        /// Writes a line to the input buffer and waits until the input is processed by the PS execution engine.
        /// </summary>
        public void ProcessInput(string line) {
            // Send the input to the command exector
            WriteInputBuffer(line);

            // Wait while the input is being processed
            SyncWaitHandle(_bufferLock);

            // Wait until the runspace is available to make sure that the command execution is finished.
            SyncWaitHandle(RunspaceLock);

            Sender(new OutputData {Finished = true, Prompt = Path + "> "});
        }

        /// <summary>
        /// Signals the session, that a single line of input queued using the <see cref="WriteInputBuffer"/> method
        /// has been processed.
        /// </summary>
        public void SignalInputProcessed() {
            _bufferLock.Set();
        }

        public CompletionData GetCommandCompletion(string command, int cursorPosition) {
            var commandCompletionPowerShell = System.Management.Automation.PowerShell.Create();
            commandCompletionPowerShell.Runspace = _runspace;
            var results = CommandCompletion.CompleteInput(command, cursorPosition, null, commandCompletionPowerShell);

            return new CompletionData {
                Results = results.CompletionMatches.Select(m => m.CompletionText).ToList(),
                CurrentMatchIndex = results.CurrentMatchIndex,
                ReplacementIndex = results.ReplacementIndex,
                ReplacementLength = results.ReplacementLength
            };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // Rollback any uncommitted explicit transactions
                foreach (var transactionScope in OrchardDrive.TransactionScopes.ToArray()) {
                    OrchardDrive.TransactionScopes.Remove(transactionScope);
                    CleanupExplicitWorkContextScope(transactionScope.Value);
                }

                if (Runspace != null) {
                    Runspace.Dispose();
                }

                if (RunspaceLock != null) {
                    _runspaceLock.Dispose();
                }
            }
        }

        protected virtual void OnDataReceived(Console.DataReceivedEventArgs e) {
            var handler = DataReceived;
            if (handler != null) {
                handler(this, e);
            }
        }

        private void OnRunspaceStateChanged(object sender, RunspaceStateEventArgs e) {
            // Initialize the current path after the runspace is opened.
            if (e.RunspaceStateInfo.State == RunspaceState.Opened) {
                Path = Runspace.SessionStateProxy.Path.CurrentLocation.ToString();
            }
        }

        private static void SyncWaitHandle(EventWaitHandle handle) {
            bool acquired = false;
            try {
                acquired = handle.WaitOne();
            }
            finally {
                if (acquired) {
                    handle.Set();
                }
            }
        }

        private void CleanupExplicitWorkContextScope(IWorkContextScope scope) {
            try {
                ITransactionManager transactionManager;
                if (scope.TryResolve(out transactionManager)) {
                    transactionManager.Cancel();
                }
            }
            catch (Exception ex) {
                Trace.WriteLine(ex.Message);
            }

            try {
                scope.Dispose();
            }
            catch (Exception ex) {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}