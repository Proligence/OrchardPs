using System;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Threading;
using Autofac;
using Orchard.Validation;
using Proligence.PowerShell.Provider.Console.UI;

namespace Proligence.PowerShell.Provider.Console.Host {
    public class ConsoleHost : PSHost, IDisposable {
        private readonly Guid _instanceId;
        private readonly CultureInfo _currentCulture;
        private readonly CultureInfo _currentUiCulture;
        private readonly ConsoleHostUserInterface _ui;
        private ConsoleHostPrivateData _privateData;
        private IPsSession _session;
        private ICommandExecutor _executor;

        public ConsoleHost(IComponentContext componentContext) {
            _instanceId = Guid.NewGuid();
            _currentCulture = Thread.CurrentThread.CurrentCulture;
            _currentUiCulture = Thread.CurrentThread.CurrentUICulture;
            _privateData = new ConsoleHostPrivateData {ComponentContext = componentContext};
            _ui = new ConsoleHostUserInterface(this);
        }

        public IPsSession Session {
            get { return _session; }
        }

        public override string Name {
            get { return "OrchardConsoleHost"; }
        }

        public override Version Version {
            get { return GetType().Assembly.GetName().Version; }
        }

        public override Guid InstanceId {
            get { return _instanceId; }
        }

        public override PSHostUserInterface UI {
            get { return _ui; }
        }

        public override CultureInfo CurrentCulture {
            get { return _currentCulture; }
        }

        public override CultureInfo CurrentUICulture {
            get { return _currentUiCulture; }
        }

        public override PSObject PrivateData {
            get { return PSObject.AsPSObject(_privateData); }
        }

        /// <summary>
        /// Attaches the console host to the specified PowerShell session.
        /// </summary>
        // ReSharper disable once ParameterHidesMember
        public void AttachToSession(IPsSession session) {
            Argument.ThrowIfNull(session, "session");

            _session = session;

            // Move the Orchard drive instance to the PowerShell session and clear the host's private data, so that
            // the session cannot be accessed directly from PowerShell scripts.
            _session.OrchardDrive = _privateData.OrchardDrive;
            _privateData = null;

            _executor = new CommandExecutor(_session);
            _executor.Start();
        }

        /// <summary>
        /// When overridden in a derived class, requests to end the current runspace. The Windows PowerShell engine
        /// calls this method to request that the host application shut down and terminate the host root runspace.
        /// </summary>
        /// <param name="exitCode">The exit code that is used to set the host's process exit code.</param>
        public override void SetShouldExit(int exitCode) {
            _executor.Exit();
            _session.Runspace.Close();
        }

        /// <summary>
        /// When overridden in a derived class, instructs the host to interrupt the currently running pipeline and
        /// start a new nested input loop.
        /// </summary>
        public override void EnterNestedPrompt() {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, instructs the host to exit the currently running input loop.
        /// </summary>
        public override void ExitNestedPrompt() {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, notifies the host that the Windows PowerShell runtime is about to
        /// execute a legacy command-line application. A legacy application is defined as a console-mode executable
        /// that can perform any of the following operations: read from stdin, write to stdout, write to stderr, or
        /// use any of the Windows console functions.
        /// </summary>
        public override void NotifyBeginApplication() {
        }

        /// <summary>
        /// When overridden in a derived class, notifies the host that the Windows PowerShell engine has completed
        /// the execution of a legacy command. A legacy application is defined as a console-mode executable that can
        /// perform any of the following operations: read from stdin, write to stdout, write to stderr, or use any of
        /// the Windows console functions.
        /// </summary>
        public override void NotifyEndApplication() {
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
                _session.Dispose();
            }
        }
    }
}