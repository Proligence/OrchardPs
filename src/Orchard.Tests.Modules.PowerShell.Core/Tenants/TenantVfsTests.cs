namespace Orchard.Tests.Modules.PowerShell.Core.Tenants
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class TenantVfsTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public TenantVfsTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainTenantSettings()
        {
            var table = this.powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            Assert.Equal(1, table.Rows.Count(x => x[0] == "BaseUrl"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "HomePage"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "PageSize"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "SiteName"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "SuperUser"));
        }

        [Fact, Integration]
        public void GetItemShouldRetrieveTenantSettingValue()
        {
            var table = this.powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            int pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);

            table = this.powerShell.ExecuteTable("Get-Item Orchard:\\Tenants\\Default\\Settings\\PageSize");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal(pageSize.ToString(CultureInfo.InvariantCulture), table.Rows.First()[1]);
        }

        [Fact, Integration]
        public void SetItemShouldUpdateTenantSettingValue()
        {
            var table = this.powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            int pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);

            int newPageSize = pageSize + 1;
            this.powerShell.Execute("Set-Item Orchard:\\Tenants\\Default\\Settings\\PageSize " + newPageSize);

            table = this.powerShell.ExecuteTable("Get-Item Orchard:\\Tenants\\Default\\Settings\\PageSize");
            pageSize = Convert.ToInt32(table.Rows.First()[1], CultureInfo.InvariantCulture);
            Assert.Equal(newPageSize, pageSize);

            table = this.powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);
            Assert.Equal(newPageSize, pageSize);
        }
    }
}