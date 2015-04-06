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
            this.powerShell.Session.ProcessInput("Get-OrchardPsCommand");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());
            Assert.Contains("Get-OrchardPsCommand", output);
        }

        [Fact, Integration]
        public void ShouldListAllCommands()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardPsCommand -All");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());
            Assert.Contains("Get-OrchardPsCommand", output);
        }

        [Fact, Integration]
        public void ShouldListCommandsForPath()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardPsCommand -Path Tenants\\Default\\Commands");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Invoke-OrchardCommand", table[0, "Name"]);
        }

        [Theory, Integration]
        [InlineData("Get-OrchardPsCommand Get*")]
        [InlineData("Get-OrchardPsCommand -Name Get*")]
        public void ShouldFilterCommands(string command)
        {
            this.powerShell.Session.ProcessInput(command);

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Assert.True(table[i, "Name"].StartsWith("Get"));
            }
        }
    }
}