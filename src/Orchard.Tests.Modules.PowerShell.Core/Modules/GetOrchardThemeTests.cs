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
            this.powerShell.Session.ProcessInput("Get-OrchardTheme");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            var row = table.Rows.Single(r => r[0] == "TheThemeMachine");
            Assert.Equal("The Theme Machine", row[1]);
            Assert.Equal("The Theme Machine", row[2]);
        }

        [Fact, Integration]
        public void ShouldGetThemeById()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardTheme TheThemeMachine");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("TheThemeMachine", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldGetThemeByName()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardTheme -Name 'The Theme Machine'");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("TheThemeMachine", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldGetThemesByWildcardId()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardTheme The*");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.True(row[0].StartsWith("The", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact, Integration]
        public void ShouldGetThemesByWildcardName()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardTheme -Name The*");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count > 0);

            foreach (var row in table.Rows)
            {
                Assert.True(row[1].StartsWith("The", StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact, Integration]
        public void ShouldGetEnabledTheme()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardTheme -Enabled");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count);
            Assert.Equal("True", table[0, "Activated"]);
        }

        [Fact, Integration]
        public void ShouldGetDisabledThemes()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardTheme -Disabled");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

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
            this.powerShell.Session.ProcessInput("Get-OrchardTheme -Tenant Default");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            var row = table.Rows.Single(r => r[0] == "TheThemeMachine");
            Assert.Equal("The Theme Machine", row[1]);
        }

        [Fact, Integration]
        public void ShouldGetThemesFromSpecificTenantByObject()
        {
            this.powerShell.Session.ProcessInput("Get-Tenant Default | Get-OrchardTheme");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            var row = table.Rows.Single(r => r[0] == "TheThemeMachine");
            Assert.Equal("The Theme Machine", row[1]);
        }

        [Fact, Integration]
        public void ShouldGetThemesFromAllTenants()
        {
            this.powerShell.Session.ProcessInput("Get-OrchardTheme -AllTenants");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count(r => r[0] == "TheThemeMachine") >= 1);
        }
    }
}