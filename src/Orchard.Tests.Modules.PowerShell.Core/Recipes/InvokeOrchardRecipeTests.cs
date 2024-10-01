using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Recipes {
    [Collection("PowerShell")]
    public class InvokeOrchardRecipeTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public InvokeOrchardRecipeTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldInvokeRecipeByName() {
            _powerShell.Execute("Invoke-OrchardRecipe \"PowerShell Sample\"");
        }

        [Fact, Integration]
        public void ShouldInvokeRecipeByObject() {
            _powerShell.Execute("Get-OrchardRecipe \"PowerShell Sample\" | Invoke-OrchardRecipe");
        }
    }
}