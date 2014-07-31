using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;
using Proligence.PowerShell.Provider.Console.UI;

namespace Proligence.PowerShell.Provider.Console.Host
{
    public class ConsoleHost : PSHost, IDisposable
    {
        private readonly RunspaceConfiguration configuration;
        private readonly Guid instanceId;
        private readonly CultureInfo currentCulture;
        private readonly CultureInfo currentUiCulture;
        private readonly PSHostUserInterface ui;
        private IPsSession session;

        public ConsoleHost(RunspaceConfiguration configuration)
        {
            this.instanceId = Guid.NewGuid();
            this.configuration = configuration;
            this.currentCulture = Thread.CurrentThread.CurrentCulture;
            this.currentUiCulture = Thread.CurrentThread.CurrentUICulture;
            this.ui = new ConsoleHostUserInterface(this);
        }

        public IPsSession Session 
        {
            get { return session; }
            
            internal set 
            {
                session = value;

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
                return "OrchardConsoleHost";
            }
        }

        public override Version Version
        {
            get
            {
                return GetType().Assembly.GetName().Version;
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

        /// <summary>
        /// When overridden in a derived class, requests to end the current runspace. The Windows PowerShell engine
        /// calls this method to request that the host application shut down and terminate the host root runspace.
        /// </summary>
        /// <param name="exitCode">The exit code that is used to set the host's process exit code.</param>
        public override void SetShouldExit(int exitCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, instructs the host to interrupt the currently running pipeline and
        /// start a new nested input loop.
        /// </summary>
        public override void EnterNestedPrompt()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, instructs the host to exit the currently running input loop.
        /// </summary>
        public override void ExitNestedPrompt()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// When overridden in a derived class, notifies the host that the Windows PowerShell runtime is about to
        /// execute a legacy command-line application. A legacy application is defined as a console-mode executable
        /// that can perform any of the following operations: read from stdin, write to stdout, write to stderr, or
        /// use any of the Windows console functions.
        /// </summary>
        public override void NotifyBeginApplication()
        {
        }

        /// <summary>
        /// When overridden in a derived class, notifies the host that the Windows PowerShell engine has completed
        /// the execution of a legacy command. A legacy application is defined as a console-mode executable that can
        /// perform any of the following operations: read from stdin, write to stdout, write to stderr, or use any of
        /// the Windows console functions.
        /// </summary>
        public override void NotifyEndApplication()
        {
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
        }
    }
}