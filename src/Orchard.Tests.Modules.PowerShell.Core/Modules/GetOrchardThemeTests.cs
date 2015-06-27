namespace Orchard.Tests.Modules.PowerShell.Core.Modules
{
    using System;
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetOrchardThemeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetOrchardThemeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllThemes()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardTheme");
            var row = table.Rows.Single(r => r[0] == "TheThemeMachine");
            Assert.Equal("The Theme Machine", row[1]);
            Assert.Equal("The Theme Machine", row[2]);
        }

        [Fact, Integration]
        public void ShouldGetThemeById()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardTheme TheThemeMachine");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("TheThemeMachine", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldGetThemeByName()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardTheme -Name 'The Theme Machine'");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("TheThemeMachine", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldGetThemesByWildcardId()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardTheme The*");
            Assert.True(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.True(row[0].StartsWith("The", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact, Integration]
        public void ShouldGetThemesByWildcardName()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardTheme -Name The*");
            Assert.True(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.True(row[1].StartsWith("The", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact, Integration]
        public void ShouldGetEnabledTheme()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardTheme -Enabled");
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("True", table[0, "Activated"]);
        }

        [Fact, Integration]
        public void ShouldGetDisabledThemes()
        {
            var output = this.powerShell.Execute("Get-OrchardTheme -Disabled");
            
            // NOTE: Output will be empty if there are no disabled themes
            if (!string.IsNullOrEmpty(output))
            {
                var table = PsTable.Parse(output);
                foreach (var row in table.Rows)
                {
                    Assert.Equal("False", row[2]);
                }
            }
        }

        [Fact, Integration]
        public void ShouldGetThemesFromSpecificTenant()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardTheme -Tenant Default");
            var row = table.Rows.Single(r => r[0] == "TheThemeMachine");
            Assert.Equal("The Theme Machine", row[1]);
        }

        [Fact, Integration]
        public void ShouldGetThemesFromSpecificTenantByObject()
        {
            var table = this.powerShell.ExecuteTable("Get-Tenant Default | Get-OrchardTheme");
            var row = table.Rows.Single(r => r[0] == "TheThemeMachine");
            Assert.Equal("The Theme Machine", row[1]);
        }

        [Fact, Integration]
        public void ShouldGetThemesFromAllTenants()
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardTheme -AllTenants");
            Assert.True(table.Rows.Count(r => r[0] == "TheThemeMachine") >= 1);
        }
    }
}