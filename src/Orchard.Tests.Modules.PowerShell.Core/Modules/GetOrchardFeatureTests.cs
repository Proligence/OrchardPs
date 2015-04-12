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
            this.powerShell.Session.ProcessInput("Get-OrchardFeature");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            var dashboardRow = table.Rows.Single(r => r[0] == "Dashboard");
            Assert.Equal("Dashboard", dashboardRow[1]);
            Assert.Equal("Core", dashboardRow[2]);
            Assert.Equal("Core", dashboardRow[4]);
            Assert.NotEmpty(dashboardRow[5]);
            Assert.NotEmpty(dashboardRow[6]);
        }

        [Fact, Integration]
        public void ShouldGetFeatureById()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardFeature Orchard.Blogs");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Orchard.Blogs", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldGetFeatureByName()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardFeature -Name Blogs");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("Orchard.Blogs", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldGetFeaturesByWildcardId()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardFeature Se*");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.True(row[0].StartsWith("Se", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact, Integration]
        public void ShouldGetFeaturesByWildcardName()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardFeature -Name Se*");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.True(row[1].StartsWith("Se", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact, Integration]
        public void ShouldGetEnabledFeatures()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardFeature -Enabled");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void ShouldGetDisabledFeatures()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardFeature -Disabled");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal(0, table.Rows.Count(r => r[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void ShouldGetFeaturesFromSpecificTenant()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardFeature -Tenant Default");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void ShouldGetFeaturesFromSpecificTenantByObject()
        {
            this.powerShell.Session.ProcessInput("Get-Tenant Default | Get-OrchardFeature");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Dashboard"));
        }

        [Fact, Integration]
        public void ShouldGetFeaturesFromAllTenants()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardFeature -FromAllTenants");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count(r => r[0] == "Dashboard") >= 1);
        }
    }
}