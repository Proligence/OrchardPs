namespace Orchard.Tests.Modules.PowerShell.Recipes
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
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());
        }

        [Fact, Integration]
        public void ShouldInvokeRecipeByObject()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardRecipe \"PowerShell Sample\" | Invoke-OrchardRecipe");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());
        }
    }
}