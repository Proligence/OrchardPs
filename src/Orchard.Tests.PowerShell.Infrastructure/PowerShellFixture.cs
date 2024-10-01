using System;
using System.IO;
using OrchardPs.Host;
using Proligence.PowerShell.Provider;
using Xunit;

namespace Orchard.Tests.PowerShell.Infrastructure {
    public class PowerShellFixture {
        private static readonly OrchardHostContextProvider _hostContextProvider;
        private static readonly TestConsoleConnection _consoleConnection;
        private static readonly IPsSession _session;

        static PowerShellFixture() {
            string orchardDir = FindOrchardWebDirectory();
            _consoleConnection = new TestConsoleConnection();
            _hostContextProvider = new OrchardHostContextProvider(orchardDir);

            var hostContext = InitializeOrchardHost();
            _session = hostContext.Session;
        }

        public IPsSession Session {
            get { return _session; }
        }

        public TestConsoleConnection ConsoleConnection {
            get { return _consoleConnection; }
        }

        public string Execute(string command) {
            Session.ProcessInput(command);
            ConsoleConnection.AssertNoErrors();

            return ConsoleConnection.Output.ToString().Trim();
        }

        public PsTable ExecuteTable(string command) {
            ConsoleConnection.Reset();
            Session.ProcessInput(command);
            ConsoleConnection.AssertNoErrors();

            string output = ConsoleConnection.Output.ToString();

            try {
                return PsTable.Parse(output);
            }
            catch (Exception ex) {
                Assert.True(false, "Failed to parse table: " + ex.Message + Environment.NewLine + output);
                throw;
            }
        }

        private static OrchardHostContext InitializeOrchardHost() {
            OrchardHostContext context = _hostContextProvider.CreateContext(_consoleConnection);
            if (context.Session == null) {
                context = _hostContextProvider.CreateContext(_consoleConnection);
            }
            else if (context.Session == null) {
                _hostContextProvider.Shutdown(context);
                throw new ApplicationException("Failed to initialize Orchard session.");
            }

            return context;
        }

        private static string FindOrchardWebDirectory() {
            // ReSharper disable once AssignNullToNotNullAttribute
            var directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
            while ((directoryInfo != null) && (directoryInfo.Name != "src")) {
                directoryInfo = directoryInfo.Parent;
            }

            if (directoryInfo == null) {
                throw new ApplicationException("Failed to find Orchard.Web directory.");
            }

            return directoryInfo.FullName + "\\Orchard.Web";
        }
    }
}