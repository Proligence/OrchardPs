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
            this.powerShell.Session.ProcessInput("Get-Tenant");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetTenantByName()
        {
            this.powerShell.Session.ProcessInput("Get-Tenant Default");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetTenantByWildcardName()
        {
            this.powerShell.Session.ProcessInput("Get-Tenant Def*");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetEnabledTenants()
        {
            this.powerShell.Session.ProcessInput("Get-Tenant -Enabled");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal("Running", table.Rows.Single(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void ShouldGetDisabledTenants()
        {
            this.powerShell.Session.ProcessInput("Get-Tenant -Disabled");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            // NOTE: When there are no disabled tenants, the output will be empty.
            if (!string.IsNullOrEmpty(output))
            {
                var table = PsTable.Parse(output);
                Assert.Equal(0, table.Rows.Count(x => x[0] == "Default"));
            }
        }
    }
}