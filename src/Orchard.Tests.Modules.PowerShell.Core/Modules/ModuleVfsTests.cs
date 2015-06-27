namespace Orchard.Tests.Modules.PowerShell.Core.Modules
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class ModuleVfsTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public ModuleVfsTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainModules()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Modules");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("ExtensionType", table.Header[1]);
            Assert.Equal("Version", table.Header[2]);
            Assert.Equal("Features", table.Header[3]);
            Assert.Equal("Description", table.Header[4]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Dashboard"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Modules"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Navigation"));
        }

        [Fact, Integration]
        public void VfsTenantShouldContainFeatures()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Features");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Id", table.Header[0]);
            Assert.Equal("Name", table.Header[1]);
            Assert.Equal("Category", table.Header[2]);
            Assert.Equal("Enabled", table.Header[3]);
            Assert.Equal("Dependencies", table.Header[4]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Common"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Contents"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void VfsTenantShouldContainThemes()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Themes");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Id", table.Header[0]);
            Assert.Equal("Name", table.Header[1]);
            Assert.Equal("Module", table.Header[2]);
            Assert.Equal("Activated", table.Header[3]);
            Assert.Equal("NeedsUpdate", table.Header[4]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "TheThemeMachine"));
        }
    }
}