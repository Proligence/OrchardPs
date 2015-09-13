using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Commands {
    [Collection("PowerShell")]
    public class InvokeOrchardCommandTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public InvokeOrchardCommandTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldInvokeLegacyCommand() {
            var output = _powerShell.Execute("Invoke-OrchardCommand help commands");
            Assert.Contains("List of available commands:", output);
        }

        [Fact, Integration]
        public void ShouldInvokeLegacyCommandWithParameter() {
            var output = _powerShell.Execute("Invoke-OrchardCommand theme list /Summary:true");
            Assert.Contains("The Theme Machine", output);
        }

        [Fact, Integration]
        public void ShouldInvokeLegacyCommandFromObject() {
            var output = _powerShell.Execute("Get-ChildItem 'Tenants\\Default\\Commands\\help commands' | Invoke-OrchardCommand");
            Assert.Contains("List of available commands:", output);
        }
    }
}