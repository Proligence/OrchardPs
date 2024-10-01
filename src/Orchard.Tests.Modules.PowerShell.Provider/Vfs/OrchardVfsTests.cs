using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Provider.Vfs {
    [Collection("PowerShell")]
    public class OrchardVfsTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public OrchardVfsTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsRootShouldContainTenants() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Tenants"));
        }

        [Fact, Integration]
        public void VfsShouldContainDefaultTenant() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("State", table.Header[1]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Default"));
            Assert.Equal("Running", table.Rows.First(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void VfsShouldListTenantContents() {
            Assert.NotEmpty(_powerShell.Execute("Get-ChildItem Orchard:\\Tenants\\Default"));
        }

        [Fact, Integration]
        public void VfsShouldContainRootDefaultTenant() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "$"));
        }

        [Fact, Integration]
        public void VfsShouldListRootDefaultTenantContents() {
            Assert.NotEmpty(_powerShell.Execute("Get-ChildItem Orchard:\\$"));
        }

        [Fact, Integration]
        public void VfsShouldUpdateCurrentLocationDisplay() {
            _powerShell.Execute("cd \\");
            Assert.Equal("Orchard:\\> ", GetCurrentLocationPrompt());
            _powerShell.Execute("cd Tenants");
            Assert.Equal("Orchard:\\Tenants> ", GetCurrentLocationPrompt());
            _powerShell.Execute("cd Default");
            Assert.Equal("Orchard:\\Tenants\\Default> ", GetCurrentLocationPrompt());
            _powerShell.Execute("cd Content");
            Assert.Equal("Orchard:\\Tenants\\Default\\Content> ", GetCurrentLocationPrompt());
            _powerShell.Execute("cd Items");
            Assert.Equal("Orchard:\\Tenants\\Default\\Content\\Items> ", GetCurrentLocationPrompt());
            _powerShell.Execute("cd Layer");
            Assert.Equal("Orchard:\\Tenants\\Default\\Content\\Items\\Layer> ", GetCurrentLocationPrompt());
        }

        private string GetCurrentLocationPrompt() {
            return _powerShell.ConsoleConnection.LastOutputData.Prompt;
        }
    }
}