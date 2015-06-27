namespace Orchard.Tests.PowerShell.Infrastructure
{
    using System;
    using System.IO;
    using OrchardPs.Host;
    using Proligence.PowerShell.Provider;
    using Xunit;

    public class PowerShellFixture
    {
        private static readonly OrchardHostContextProvider hostContextProvider;
        private static readonly TestConsoleConnection consoleConnection;
        private static readonly IPsSession session;

        static PowerShellFixture()
        {
            string orchardDir = FindOrchardWebDirectory();
            consoleConnection = new TestConsoleConnection();
            hostContextProvider = new OrchardHostContextProvider(orchardDir);
            
            var hostContext = InitializeOrchardHost();
            session = hostContext.Session;
        }

        public IPsSession Session
        {
            get { return session; }
        }

        public TestConsoleConnection ConsoleConnection
        {
            get { return consoleConnection; }
        }

        public string Execute(string command)
        {
            this.Session.ProcessInput(command);
            this.ConsoleConnection.AssertNoErrors();

            return this.ConsoleConnection.Output.ToString().Trim();
        }

        public PsTable ExecuteTable(string command)
        {
            this.ConsoleConnection.Reset();
            this.Session.ProcessInput(command);
            this.ConsoleConnection.AssertNoErrors();

            string output = this.ConsoleConnection.Output.ToString();

            try
            {
                return PsTable.Parse(output);
            }
            catch (Exception ex)
            {
                Assert.True(false, "Failed to parse table: " + ex.Message + Environment.NewLine + output);
                throw;
            }
        }

        private static OrchardHostContext InitializeOrchardHost()
        {
            OrchardHostContext context = hostContextProvider.CreateContext(consoleConnection);
            if (context.Session == null)
            {
                context = hostContextProvider.CreateContext(consoleConnection);
            }
            else if (context.Session == null)
            {
                hostContextProvider.Shutdown(context);
                throw new ApplicationException("Failed to initialize Orchard session.");
            }

            return context;
        }

        private static string FindOrchardWebDirectory()
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