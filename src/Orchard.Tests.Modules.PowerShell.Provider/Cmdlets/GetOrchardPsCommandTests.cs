namespace Orchard.Tests.Modules.PowerShell.Provider.Cmdlets
{
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetOrchardPsCommandTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetOrchardPsCommandTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldListCommands()
        {
            Assert.Contains("Get-OrchardPsCommand", this.powerShell.Execute("Get-OrchardPsCommand"));
        }

        [Fact, Integration]
        public void ShouldListAllCommands()
        {
            Assert.Contains("Get-OrchardPsCommand", this.powerShell.Execute("Get-OrchardPsCommand -All"));
        }

        [Fact, Integration]
        public void ShouldListCommandsForPath()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardPsCommand -Path Tenants\\Default\\Commands");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Invoke-OrchardCommand", table[0, "Name"]);
        }

        [Theory, Integration]
        [InlineData("Get-OrchardPsCommand Get*")]
        [InlineData("Get-OrchardPsCommand -Name Get*")]
        public void ShouldFilterCommands(string command)
        {
            var table = this.powerShell.ExecuteTable(command);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Assert.True(table[i, "Name"].StartsWith("Get"));
            }
        }
    }
}