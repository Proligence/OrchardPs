namespace Orchard.Tests.Modules.PowerShell.Core.Recipes
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetOrchardRecipeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetOrchardRecipeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllRecipes()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardRecipe");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Version", table.Header[1]);
            Assert.Equal("Description", table.Header[2]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Blog"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Core"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Default"));
        }

        [Fact, Integration]
        public void ShouldGetRecipesByName()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardRecipe Core");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Version", table.Header[1]);
            Assert.Equal("Description", table.Header[2]);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Core", table[0, "Name"]);
        }

        [Fact, Integration]
        public void ShouldGetRecipesByPartialName()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardRecipe C*");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Version", table.Header[1]);
            Assert.Equal("Description", table.Header[2]);
            Assert.Equal(1, table.Rows.Count);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Core"));
            Assert.All(table.Rows, r => Assert.StartsWith("C", r[0]));
        }

        [Fact, Integration]
        public void ShouldGetRecipesByExtensionId()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardRecipe -ExtensionId Orchard.Setup");
            Assert.Equal(3, table.Rows.Count);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Blog"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Core"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Default"));
        }
    }
}