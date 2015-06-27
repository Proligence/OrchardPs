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
            this.powerShell.Session.ProcessInput("Get-OrchardRecipe");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
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
            this.powerShell.Session.ProcessInput("Get-OrchardRecipe Core");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Version", table.Header[1]);
            Assert.Equal("Description", table.Header[2]);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Core", table[0, "Name"]);
        }

        [Fact, Integration]
        public void ShouldGetRecipesByPartialName()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardRecipe C*");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
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
            this.powerShell.Session.ProcessInput("Get-OrchardRecipe -ExtensionId Orchard.Setup");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal(3, table.Rows.Count);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Blog"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Core"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Default"));
        }
    }
}