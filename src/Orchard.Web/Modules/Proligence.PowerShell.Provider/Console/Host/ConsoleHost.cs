using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;
using Proligence.PowerShell.Provider.Console.UI;

namespace Proligence.PowerShell.Provider.Console.Host
{
    public class ConsoleHost : PSHost, IDisposable, IHostSupportsInteractiveSession
    {
        private readonly RunspaceConfiguration configuration;
        private readonly Guid instanceId;
        private readonly CultureInfo currentCulture;
        private readonly CultureInfo currentUiCulture;
        private readonly PSHostUserInterface ui;
        private IPsSession _session;

        public ConsoleHost(RunspaceConfiguration configuration)
        {
            this.instanceId = Guid.NewGuid();
            this.configuration = configuration;
            this.currentCulture = Thread.CurrentThread.CurrentCulture;
            this.currentUiCulture = Thread.CurrentThread.CurrentUICulture;
            this.ui = new ConsoleHostUserInterface(this);
        }

        public IPsSession Session {
            get { return _session; }
            internal set {
                _session = value;

                // Echo testing code
                //---------------------------
                //_session.DataReceived += (sender, args) => {
                //    this.ui.Write(ConsoleColor.DarkBlue, ConsoleColor.Cyan, "A oto Twój tekst: ");
                //    this.ui.WriteLine(ConsoleColor.Black, ConsoleColor.Green, ((IPsSession) sender).ReadInputBuffer());
                //    this.ui.WriteErrorLine("This is an error!");
                //};
            }
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
                return typeof(ConsoleHost).Assembly.GetName().Version;
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