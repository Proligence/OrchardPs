namespace Orchard.Tests.Modules.PowerShell.Core.Recipes
{
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class InvokeOrchardRecipeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public InvokeOrchardRecipeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldInvokeRecipeByName()
        {
            this.powerShell.Execute("Invoke-OrchardRecipe \"PowerShell Sample\"");
        }

        [Fact, Integration]
        public void ShouldInvokeRecipeByObject()
        {
            this.powerShell.Execute("Get-OrchardRecipe \"PowerShell Sample\" | Invoke-OrchardRecipe");
        }
    }
}