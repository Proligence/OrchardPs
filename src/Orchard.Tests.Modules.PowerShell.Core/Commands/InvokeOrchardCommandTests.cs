namespace Orchard.Tests.Modules.PowerShell.Core.Commands
{
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class InvokeOrchardCommandTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public InvokeOrchardCommandTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldInvokeLegacyCommand()
        {
            this.powerShell.Session.ProcessInput("Invoke-OrchardCommand help commands");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Contains("List of available commands:", output);
        }

        [Fact, Integration]
        public void ShouldInvokeLegacyCommandWithParameter()
        {
            this.powerShell.Session.ProcessInput("Invoke-OrchardCommand theme list /Summary:true");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Contains("The Theme Machine", output);
        }

        [Fact, Integration]
        public void ShouldInvokeLegacyCommandFromObject()
        {
            this.powerShell.Session.ProcessInput(
                "Get-ChildItem 'Tenants\\Default\\Commands\\help commands' | Invoke-OrchardCommand");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Contains("List of available commands:", output);
        }
    }
}