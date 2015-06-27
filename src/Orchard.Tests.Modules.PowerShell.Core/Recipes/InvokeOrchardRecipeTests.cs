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
            this.powerShell.Session.ProcessInput("Invoke-OrchardRecipe \"PowerShell Sample\"");
            this.powerShell.ConsoleConnection.AssertNoErrors();
        }

        [Fact, Integration]
        public void ShouldInvokeRecipeByObject()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardRecipe \"PowerShell Sample\" | Invoke-OrchardRecipe");
            this.powerShell.ConsoleConnection.AssertNoErrors();
        }
    }
}