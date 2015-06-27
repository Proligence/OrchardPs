namespace Orchard.Tests.Modules.PowerShell.Core.Modules
{
    using System;
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetOrchardFeatureTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetOrchardFeatureTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllFeatures()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature");
            var dashboardRow = table.Rows.Single(r => r[0] == "Dashboard");
            Assert.Equal("Dashboard", dashboardRow[1]);
        }

        [Fact, Integration]
        public void ShouldGetFeatureById()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature Orchard.Blogs");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Orchard.Blogs", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldGetFeatureByName()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature -Name Blogs");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Orchard.Blogs", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldGetFeaturesByWildcardId()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature Se*");
            Assert.True(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.True(row[0].StartsWith("Se", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact, Integration]
        public void ShouldGetFeaturesByWildcardName()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature -Name Se*");
            Assert.True(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.True(row[1].StartsWith("Se", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact, Integration]
        public void ShouldGetEnabledFeatures()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature -Enabled");
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void ShouldGetDisabledFeatures()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature -Disabled");
            Assert.Equal(0, table.Rows.Count(r => r[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void ShouldGetFeaturesFromSpecificTenant()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature -Tenant Default");
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void ShouldGetFeaturesFromSpecificTenantByObject()
        {
            var table = this.powerShell.ExecuteTable("Get-Tenant Default | Get-OrchardFeature");
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void ShouldGetFeaturesFromAllTenants()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature -AllTenants");
            Assert.True(table.Rows.Count(r => r[0] == "Dashboard") >= 1);
        }
    }
}