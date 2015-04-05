namespace Orchard.Tests.Modules.PowerShell.Provider.Cmdlets
{
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

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

        [Theory, Integration]
        [InlineData("Get-OrchardPsCommand Get*")]
        [InlineData("Get-OrchardPsCommand -Name Get*")]
        public void ShouldFilterCommands(string command)
        {
            this.powerShell.Session.ProcessInput(command);

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTestHelper.ParseTable(output);
            Assert.Equal("Name", table[0][1]);
            for (int i = 1; i < table.Count; i++)
            {
                Assert.True(table[i][1].StartsWith("Get"));
            }
        }
    }
}