using System;
using System.Globalization;
using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Tenants {
    [Collection("PowerShell")]
    public class TenantVfsTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public TenantVfsTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainTenantSettings() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            Assert.Equal(1, table.Rows.Count(x => x[0] == "BaseUrl"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "HomePage"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "PageSize"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "SiteName"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "SuperUser"));
        }

        [Fact, Integration]
        public void GetItemShouldRetrieveTenantSettingValue() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            int pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);

            table = _powerShell.ExecuteTable("Get-Item Orchard:\\Tenants\\Default\\Settings\\PageSize");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal(pageSize.ToString(CultureInfo.InvariantCulture), table.Rows.First()[1]);
        }

        [Fact, Integration]
        public void SetItemShouldUpdateTenantSettingValue() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            int pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);

            int newPageSize = pageSize + 1;
            _powerShell.Execute("Set-Item Orchard:\\Tenants\\Default\\Settings\\PageSize " + newPageSize);

            table = _powerShell.ExecuteTable("Get-Item Orchard:\\Tenants\\Default\\Settings\\PageSize");
            pageSize = Convert.ToInt32(table.Rows.First()[1], CultureInfo.InvariantCulture);
            Assert.Equal(newPageSize, pageSize);

            table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);
            Assert.Equal(newPageSize, pageSize);
        }
    }
}