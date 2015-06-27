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
            var table = this.powerShell.ExecuteTable("Get-ContentPartDefinition");
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
            var table = this.powerShell.ExecuteTable(command);
            Assert.Equal("CommonPart", table.Rows.Single()[0]);
        }

        [Theory, Integration]
        [InlineData("Get-ContentPartDefinition Co*")]
        [InlineData("Get-ContentPartDefinition -Name Co*")]
        public void ShouldGetContentPartsByWildcardName(string command)
        {
            var table = this.powerShell.ExecuteTable(command);
            Assert.Equal(1, table.Rows.Count(r => r[0] == "CommonPart"));
            Assert.True(table.Rows.All(r => r[0].StartsWith("Co")));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromSpecificTenant()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentPartDefinition -Tenant Default");
            Assert.Equal(1, table.Rows.Count(r => r[0] == "BodyPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "CommonPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "IdentityPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "ContainerPart"));
            Assert.Equal(1, table.Rows.Count(r => r[0] == "TitlePart"));
        }

        [Fact, Integration]
        public void ShouldGetContentPartsFromAllTenants()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentPartDefinition -AllTenants");
            Assert.True(table.Rows.Count(r => r[0] == "BodyPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "CommonPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "IdentityPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "ContainerPart") > 0);
            Assert.True(table.Rows.Count(r => r[0] == "TitlePart") > 0);
        }
    }
}