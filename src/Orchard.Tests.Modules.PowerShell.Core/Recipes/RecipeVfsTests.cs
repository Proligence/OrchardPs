using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Recipes {
    [Collection("PowerShell")]
    public class RecipeVfsTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public RecipeVfsTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainRecipes() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Recipes");
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