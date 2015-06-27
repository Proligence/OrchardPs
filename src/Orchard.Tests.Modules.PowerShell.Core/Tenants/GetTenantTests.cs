namespace Orchard.Tests.Modules.PowerShell.Core.Tenants
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetTenantTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetTenantTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllTenants()
        {
            var table = this.powerShell.ExecuteTable("Get-Tenant");
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetTenantByName()
        {
            var table = this.powerShell.ExecuteTable("Get-Tenant Default");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetTenantByWildcardName()
        {
            var table = this.powerShell.ExecuteTable("Get-Tenant Def*");
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetEnabledTenants()
        {
            var table = this.powerShell.ExecuteTable("Get-Tenant -Enabled");
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetDisabledTenants()
        {
            var output = this.powerShell.Execute("Get-Tenant -Disabled");
            
            // NOTE: When there are no disabled tenants, the output will be empty.
            if (!string.IsNullOrEmpty(output))
            {
                var table = PsTable.Parse(output);
                Assert.Equal(0, table.Rows.Count(x => x[0] == "Default"));
            }
        }
    }
}