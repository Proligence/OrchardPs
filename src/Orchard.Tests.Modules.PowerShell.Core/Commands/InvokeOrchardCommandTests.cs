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
            var output = this.powerShell.Execute("Invoke-OrchardCommand help commands");
            Assert.Contains("List of available commands:", output);
        }

        [Fact, Integration]
        public void ShouldInvokeLegacyCommandWithParameter()
        {
            var output = this.powerShell.Execute("Invoke-OrchardCommand theme list /Summary:true");
            Assert.Contains("The Theme Machine", output);
        }

        [Fact, Integration]
        public void ShouldInvokeLegacyCommandFromObject()
        {
            var output = this.powerShell.Execute("Get-ChildItem 'Tenants\\Default\\Commands\\help commands' | Invoke-OrchardCommand");
            Assert.Contains("List of available commands:", output);
        }
    }
}