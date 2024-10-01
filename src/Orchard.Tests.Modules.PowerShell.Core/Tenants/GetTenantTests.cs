using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Tenants {
    [Collection("PowerShell")]
    public class GetTenantTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public GetTenantTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllTenants() {
            var table = _powerShell.ExecuteTable("Get-Tenant");
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetTenantByName() {
            var table = _powerShell.ExecuteTable("Get-Tenant Default");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetTenantByWildcardName() {
            var table = _powerShell.ExecuteTable("Get-Tenant Def*");
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetEnabledTenants() {
            var table = _powerShell.ExecuteTable("Get-Tenant -Enabled");
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetDisabledTenants() {
            var output = _powerShell.Execute("Get-Tenant -Disabled");

            // NOTE: When there are no disabled tenants, the output will be empty.
            if (!string.IsNullOrEmpty(output)) {
                var table = PsTable.Parse(output);
                Assert.Equal(0, table.Rows.Count(x => x[0] == "Default"));
            }
        }
    }
}