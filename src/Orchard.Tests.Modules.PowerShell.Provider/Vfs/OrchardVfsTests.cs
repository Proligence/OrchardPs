namespace Orchard.Tests.Modules.PowerShell.Provider.Vfs
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class OrchardVfsTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public OrchardVfsTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsRootShouldContainTenants()
        {
            var table = this.powerShell.ExecuteTable("Get-ChildItem Orchard:\\");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Tenants"));
        }

        [Fact, Integration]
        public void VfsShouldContainDefaultTenant()
        {
            var table = this.powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("State", table.Header[1]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Default"));
            Assert.Equal("Running", table.Rows.First(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void VfsShouldListTenantContents()
        {
            Assert.NotEmpty(this.powerShell.Execute("Get-ChildItem Orchard:\\Tenants\\Default"));
        }

        [Fact, Integration]
        public void VfsShouldContainRootDefaultTenant()
        {
            var table = this.powerShell.ExecuteTable("Get-ChildItem Orchard:\\");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "$"));
        }

        [Fact, Integration]
        public void VfsShouldListRootDefaultTenantContents()
        {
            Assert.NotEmpty(this.powerShell.Execute("Get-ChildItem Orchard:\\$"));
        }

        [Fact, Integration]
        public void VfsShouldUpdateCurrentLocationDisplay()
        {
            this.powerShell.Execute("cd \\");
            Assert.Equal("Orchard:\\> ", this.GetCurrentLocationPrompt());
            this.powerShell.Execute("cd Tenants");
            Assert.Equal("Orchard:\\Tenants> ", this.GetCurrentLocationPrompt());
            this.powerShell.Execute("cd Default");
            Assert.Equal("Orchard:\\Tenants\\Default> ", this.GetCurrentLocationPrompt());
            this.powerShell.Execute("cd Content");
            Assert.Equal("Orchard:\\Tenants\\Default\\Content> ", this.GetCurrentLocationPrompt());
            this.powerShell.Execute("cd Items");
            Assert.Equal("Orchard:\\Tenants\\Default\\Content\\Items> ", this.GetCurrentLocationPrompt());
            this.powerShell.Execute("cd Layer");
            Assert.Equal("Orchard:\\Tenants\\Default\\Content\\Items\\Layer> ", this.GetCurrentLocationPrompt());
        }

        private string GetCurrentLocationPrompt()
        {
            return this.powerShell.ConsoleConnection.LastOutputData.Prompt;
        }
    }
}