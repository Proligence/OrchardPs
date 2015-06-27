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
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "BaseUrl"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "HomePage"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "PageSize"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "SiteName"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "SuperUser"));
        }

        [Fact, Integration]
        public void GetItemShouldRetrieveTenantSettingValue()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            int pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);
            
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-Item Orchard:\\Tenants\\Default\\Settings\\PageSize");
            table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal(pageSize.ToString(CultureInfo.InvariantCulture), table.Rows.First()[1]);
        }

        [Fact, Integration]
        public void SetItemShouldUpdateTenantSettingValue()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            int pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);

            int newPageSize = pageSize + 1;
            this.powerShell.Session.ProcessInput("Set-Item Orchard:\\Tenants\\Default\\Settings\\PageSize " + newPageSize);

            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-Item Orchard:\\Tenants\\Default\\Settings\\PageSize");
            table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            pageSize = Convert.ToInt32(table.Rows.First()[1], CultureInfo.InvariantCulture);
            Assert.Equal(newPageSize, pageSize);

            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Settings");
            table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            pageSize = Convert.ToInt32(table.Rows.First(x => x[0] == "PageSize")[1], CultureInfo.InvariantCulture);
            Assert.Equal(newPageSize, pageSize);
        }
    }
}