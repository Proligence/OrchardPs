namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Threading;
    using Autofac;
    using Console;
    using Proligence.PowerShell.Provider.Console.Host;
    using Proligence.PowerShell.Provider.Console.UI;
    using Proligence.PowerShell.Provider.Vfs;

    /// <summary>
    /// Represents a PowerShell user session.
    /// </summary>
    public class PsSession : IPsSession 
    {
        private readonly ConcurrentQueue<string> queue;
        private readonly RunspaceConfiguration configuration;
        private readonly IComponentContext componentContext;
        private readonly Runspace runspace;
        private readonly AutoResetEvent runspaceLock;
        
        /// <summary>
        /// Caches the current path of the session's runspace. This cached value is used if the runspace cannot be
        /// accessed because a cmdlet is being executed in it.
        /// </summary>
        private string currentPath;

        public PsSession(
            ConsoleHost consoleHost,
            RunspaceConfiguration configuration,
            IComponentContext componentContext,
            string connectionId)
        {
            this.ConsoleHost = consoleHost;
            this.ConnectionId = connectionId;

            this.queue = new ConcurrentQueue<string>();
            this.configuration = configuration;
            this.componentContext = componentContext;
            this.runspace = RunspaceFactory.CreateRunspace(consoleHost, configuration);
            this.runspaceLock = new AutoResetEvent(true);
            this.Runspace.StateChanged += this.OnRunspaceStateChanged;
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Gets the console host which provides input/output to the PowerShell engine.
        /// </summary>
        public ConsoleHost ConsoleHost { get; private set; }

        /// <summary>
        /// Gets the configuration of the PowerShell runspace associated with the session.
        /// </summary>
        public RunspaceConfiguration RunspaceConfiguration
        {
            get { return this.configuration; }
        }

        /// <summary>
        /// Gets the PowerShell runspace associated with the session.
        /// </summary>
        public Runspace Runspace
        {
            get { return this.runspace; }
        }

        /// <summary>
        /// Gets the lock which must be aquired before acessing the session's runspace.
        /// </summary>
        public EventWaitHandle RunspaceLock
        {
            get { return this.runspaceLock; }
        }

        /// <summary>
        /// Gets the current session's runspace absolute path.
        /// </summary>
        public string Path
        {
            get
            {
                if (this.runspaceLock.WaitOne(0))
                {
                    try
                    {
                        this.currentPath = this.Runspace.SessionStateProxy.Path.CurrentLocation.ToString();
                    }
                    finally
                    {
                        this.runspaceLock.Set();
                    }
                }

                return this.currentPath ?? string.Empty;
            }

            set 
            {
                this.currentPath = value;
            }
        }

        /// <summary>
        /// Delegate used for sending messages up to the user console.
        /// </summary>
        public Action<OutputData> Sender { get; internal set; }

        /// <summary>
        /// Gets the dependency injection container for the Orchard application.
        /// </summary>
        public IComponentContext ComponentContext
        {
            get
            {
                return this.componentContext;
            }
        }

        /// <summary>
        /// Gets the Orchard drive instance for this session.
        /// </summary>
        public OrchardDriveInfo OrchardDrive { get; set; }

        /// <summary>
        /// Gets the session's Orchard VFS instance.
        /// </summary>
        public IPowerShellVfs Vfs
        {
            get
            {
                if (this.OrchardDrive != null)
                {
                    return this.OrchardDrive.Vfs;                    
                }

                return null;
            }
        }

        /// <summary>
        /// SignalR connection identifier for this particular session.
        /// </summary>
        public string ConnectionId { get; private set; }

        /// <summary>
        /// Initializes the session.
        /// </summary>
        public void Initialize()
        {
            // NOTE (MD):
            // For some strange reason the initilization scripts from the runtime configuration are not executed
            // while opening the runspace, so we need to execute them manually. #OPS-60
            var runspaceConfiguration = this.runspace.RunspaceConfiguration;
            var initScripts = runspaceConfiguration.InitializationScripts.ToArray();
            runspaceConfiguration.InitializationScripts.Reset();

            this.runspace.Open();

            foreach (ScriptConfigurationEntry entry in initScripts)
            {
                using (var pipeline = this.runspace.CreatePipeline(entry.Definition))
                {
                    pipeline.Commands.AddScript(entry.Definition);
                    pipeline.Invoke();
                }
            }
        }

        /// <summary>
        /// Reads line of string from input buffer. Nonblocking.
        /// </summary>
        public string ReadInputBuffer()
        {
            string result;
            if (this.queue.TryDequeue(out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Writes a line to the input buffer.
        /// </summary>
        public void WriteInputBuffer(string line)
        {
            this.queue.Enqueue(line);
            this.OnDataReceived(new DataReceivedEventArgs());
        }

        public CompletionData GetCommandCompletion(string command, int cursorPosition) 
        {
            var commandCompletionPowerShell = PowerShell.Create();
            commandCompletionPowerShell.Runspace = this.runspace;
            var results = CommandCompletion.CompleteInput(command, cursorPosition, null, commandCompletionPowerShell);

            return new CompletionData 
            {
                Results = results.CompletionMatches.Select(m => m.CompletionText).ToList(),
                CurrentMatchIndex = results.CurrentMatchIndex,
                ReplacementIndex = results.ReplacementIndex,
                ReplacementLength = results.ReplacementLength
            };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() 
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Runspace != null)
                {
                    this.Runspace.Dispose();
                }

                if (this.RunspaceLock != null)
                {
                    this.runspaceLock.Dispose();
                }
            }
        }

        protected virtual void OnDataReceived(DataReceivedEventArgs e)
        {
            var handler = this.DataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnRunspaceStateChanged(object sender, RunspaceStateEventArgs e)
        {
            // Intialize the current path after the runspace is opened.
            if (e.RunspaceStateInfo.State == RunspaceState.Opened)
            {
                this.currentPath = this.Runspace.SessionStateProxy.Path.CurrentLocation.ToString();
            }
        }
    }
}