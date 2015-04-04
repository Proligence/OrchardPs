namespace Orchard.Tests.Modules.PowerShell.Core.Infrastructure
{
    using System;
    using System.IO;
    using OrchardPs.Host;
    using Proligence.PowerShell.Provider;

    public class PowerShellFixture : IDisposable
    {
        private readonly OrchardHostContextProvider hostContextProvider;
        private readonly OrchardHostContext hostContext;
        
        public PowerShellFixture()
        {
            string orchardDir = this.FindOrchardWebDirectory();

            this.ConsoleConnection = new TestConsoleConnection();
            this.hostContextProvider = new OrchardHostContextProvider(orchardDir);
            this.hostContext = this.InitializeOrchardHost();
            this.Session = this.hostContext.Session;
        }

        public IPsSession Session { get; private set; }
        public TestConsoleConnection ConsoleConnection { get; private set; }

        public void Dispose()
        {
            this.hostContextProvider.Shutdown(this.hostContext);
        }

        private OrchardHostContext InitializeOrchardHost()
        {
            OrchardHostContext context = this.hostContextProvider.CreateContext(this.ConsoleConnection);
            if (context.Session == null)
            {
                context = this.hostContextProvider.CreateContext(this.ConsoleConnection);
            }
            else if (context.Session == null)
            {
                this.hostContextProvider.Shutdown(context);
                throw new ApplicationException("Failed to initialize Orchard session.");
            }

            return context;
        }

        private string FindOrchardWebDirectory()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
            while ((directoryInfo != null) && (directoryInfo.Name != "src"))
            {
                directoryInfo = directoryInfo.Parent;
            }

            if (directoryInfo == null)
            {
                throw new ApplicationException("Failed to find Orchard.Web directory.");
            }

            return directoryInfo.FullName + "\\Orchard.Web";
        }
    }
}