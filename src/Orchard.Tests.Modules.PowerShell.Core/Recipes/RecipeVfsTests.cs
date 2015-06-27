namespace Orchard.Tests.Modules.PowerShell.Core.Recipes
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class RecipeVfsTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public RecipeVfsTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainRecipes()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Recipes");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Version", table.Header[1]);
            Assert.Equal("Description", table.Header[2]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Blog"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Core"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Default"));
        }
    }
}