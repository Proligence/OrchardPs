namespace Proligence.PowerShell.Provider.Console
{
    using System;
    using System.Globalization;
    using System.Management.Automation.Host;
    using System.Management.Automation.Runspaces;
    using System.Threading;

    public class SignalRConsoleHost : PSHost, IDisposable, IHostSupportsInteractiveSession
    {
        private readonly RunspaceConfiguration configuration;
        private readonly Guid instanceId;
        private readonly CultureInfo currentCulture;
        private readonly CultureInfo currentUiCulture;
        private readonly PSHostUserInterface ui;

        public SignalRConsoleHost(RunspaceConfiguration configuration)
        {
            this.instanceId = Guid.NewGuid();
            this.configuration = configuration;
            this.currentCulture = Thread.CurrentThread.CurrentCulture;
            this.currentUiCulture = Thread.CurrentThread.CurrentUICulture;
            this.ui = new SignalRConsoleHostUserInterface(this);
        }

        public override string Name
        {
            get
            {
                return "Orchard Provider ConsoleHost";
            }
        }

        public override Version Version
        {
            get
            {
                return typeof(SignalRConsoleHost).Assembly.GetName().Version;
            }
        }

        public override Guid InstanceId
        {
            get { return this.instanceId; }
        }

        public override PSHostUserInterface UI
        {
            get { return this.ui; }
        }

        public override CultureInfo CurrentCulture
        {
            get { return this.currentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return this.currentUiCulture; }
        }

        public bool IsRunspacePushed { get; private set; }

        public Runspace Runspace { get; private set; }

        public override void SetShouldExit(int exitCode)
        {
            throw new NotImplementedException();
        }

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void NotifyBeginApplication()
        {
            throw new NotImplementedException();
        }

        public override void NotifyEndApplication()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void PushRunspace(Runspace runspace)
        {
            throw new NotImplementedException();
        }

        public void PopRunspace()
        {
            throw new NotImplementedException();
        }
    }
}