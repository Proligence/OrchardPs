using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Provider.Cmdlets {
    [Collection("PowerShell")]
    public class GetOrchardPsCommandTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public GetOrchardPsCommandTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldListCommands() {
            Assert.Contains("Get-OrchardPsCommand", _powerShell.Execute("Get-OrchardPsCommand"));
        }

        [Fact, Integration]
        public void ShouldListAllCommands() {
            Assert.Contains("Get-OrchardPsCommand", _powerShell.Execute("Get-OrchardPsCommand -All"));
        }

        [Fact, Integration]
        public void ShouldListCommandsForPath() {
            var table = _powerShell.ExecuteTable("Get-OrchardPsCommand -Path Tenants\\Default\\Commands");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Invoke-OrchardCommand", table[0, "Name"]);
        }

        [Theory, Integration]
        [InlineData("Get-OrchardPsCommand Get*")]
        [InlineData("Get-OrchardPsCommand -Name Get*")]
        public void ShouldFilterCommands(string command) {
            var table = _powerShell.ExecuteTable(command);
            for (int i = 0; i < table.Rows.Count; i++) {
                Assert.True(table[i, "Name"].StartsWith("Get"));
            }
        }
    }
}