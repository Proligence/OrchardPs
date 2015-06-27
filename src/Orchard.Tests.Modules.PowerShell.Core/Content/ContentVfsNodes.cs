namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class ContentVfsNodes : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public ContentVfsNodes(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainContentParts()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Parts");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Attachable", table.Header[1]);
            Assert.Equal("Fields", table.Header[2]);
            Assert.Equal("Description", table.Header[3]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "CommonPart"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "ContainerPart"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "TitlePart"));
        }

        [Fact, Integration]
        public void VfsTenantShouldContainContentTypes()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Types");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("DisplayName", table.Header[1]);
            Assert.Equal("Parts", table.Header[2]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Layer"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Site"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "User"));
        }

        [Fact, Integration]
        public void VfsTenantShouldContainContentItemTypes()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Items");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Description", table.Header[1]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Layer"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Site"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "User"));
        }

        [Fact, Integration]
        public void VfsContentItemTypeShouldContainContentItems()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Items\\Layer");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Id", table.Header[0]);
            Assert.Equal("ContentType", table.Header[1]);
            Assert.Equal("Title", table.Header[2]);
            Assert.Equal("Published", table.Header[3]);
            Assert.True(table.Rows.Count > 0);
        }

        [Fact, Integration]
        public void ContentItemsShouldSupportCustomFormatting()
        {
            this.powerShell.Session.ProcessInput(
                "Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Items\\User | where { $_.UserName -eq 'Admin' } | fc");

            var output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.NotEmpty(output);
            Assert.Contains("Content Item #", output);
            Assert.Contains("ContentType = User", output);
            Assert.Contains("UserPart (Orchard.Users.Models.UserPart)", output);
            Assert.Contains("UserName = admin", output);
        }

        [Fact, Integration]
        public void ContentItemsShouldContainPropertiesFromContentParts()
        {
            this.powerShell.Session.ProcessInput(
                "Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Items\\User | ft UserName, UserPart");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            var userRow = table.Rows.Single(x => x[0] == "admin");
            Assert.Equal("Orchard.Users.Models.UserPart", userRow[1]);
        }
    }
}