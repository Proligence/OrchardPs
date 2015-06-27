namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetContentPartDefinitionTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetContentPartDefinitionTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllContentParts()
        {
            this.powerShell.Session.ProcessInput("Get-ContentPartDefinition");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "BodyPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "CommonPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "IdentityPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "ContainerPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "TitlePart"));
        }

        [Theory, Integration]
        [InlineData("Get-ContentPartDefinition CommonPart")]
        [InlineData("Get-ContentPartDefinition -Name CommonPart")]
        public void ShouldGetContentPartsByName(string command)
        {
            this.powerShell.Session.ProcessInput(command);

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal("CommonPart", table.Rows.Single()[0]);
        }

        [Theory, Integration]
        [InlineData("Get-ContentPartDefinition Co*")]
        [InlineData("Get-ContentPartDefinition -Name Co*")]
        public void ShouldGetContentPartsByWildcardName(string command)
        {
            this.powerShell.Session.ProcessInput(command);

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "CommonPart"));
            Assert.True(table.Rows.All(r => r[0].StartsWith("Co")));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromSpecificTenant()
        {
            this.powerShell.Session.ProcessInput("Get-ContentPartDefinition -Tenant Default");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "BodyPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "CommonPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "IdentityPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "ContainerPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "TitlePart"));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromAllTenants()
        {
            this.powerShell.Session.ProcessInput("Get-ContentPartDefinition -AllTenants");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count(r => r[0] == "BodyPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "CommonPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "IdentityPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "ContainerPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "TitlePart") > 0);
        }
    }
}