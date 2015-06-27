namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetContentTypeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetContentTypeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllContentTypes()
        {
            this.powerShell.Session.ProcessInput("Get-ContentType");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Site"));
        }

        [Theory, Integration]
        [InlineData("Get-ContentType Site")]
        [InlineData("Get-ContentType -Name Site")]
        public void ShouldContentTypesByName(string command)
        {
            this.powerShell.Session.ProcessInput(command);

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal("Site", table.Rows.Single()[0]);
        }

        [Theory, Integration]
        [InlineData("Get-ContentType S*")]
        [InlineData("Get-ContentType -Name S*")]
        public void ShouldGetContentTypesByWildcardName(string command)
        {
            this.powerShell.Session.ProcessInput(command);

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Site"));
            Assert.True(table.Rows.All(r => r[0].StartsWith("S")));
        }

        [Fact, Integration]
        public void ShouldGetContentTypesFromSpecificTenant()
        {
            this.powerShell.Session.ProcessInput("Get-ContentType -Tenant Default");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "Site"));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromAllTenants()
        {
            this.powerShell.Session.ProcessInput("Get-ContentType -AllTenants");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count(r => r[0] == "Site") > 0);
        }
    }
}