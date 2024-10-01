using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class ContentVfsNodes : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public ContentVfsNodes(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainContentParts() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Parts");
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
        public void VfsTenantShouldContainContentTypes() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Types");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("DisplayName", table.Header[1]);
            Assert.Equal("Parts", table.Header[2]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Layer"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Site"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "User"));
        }

        [Fact, Integration]
        public void VfsTenantShouldContainContentItemTypes() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Items");
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Description", table.Header[1]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Layer"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Site"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "User"));
        }

        [Fact, Integration]
        public void VfsContentItemTypeShouldContainContentItems() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Items\\Layer");
            Assert.Equal("Id", table.Header[0]);
            Assert.Equal("ContentType", table.Header[1]);
            Assert.Equal("Title", table.Header[2]);
            Assert.Equal("Published", table.Header[3]);
            Assert.True(table.Rows.Count > 0);
        }

        [Fact, Integration]
        public void ContentItemsShouldSupportCustomFormatting() {
            var output = _powerShell.Execute(
                "Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Items\\User | where { $_.UserName -eq 'Admin' } | fc");

            Assert.NotEmpty(output);
            Assert.Contains("Content Item #", output);
            Assert.Contains("ContentType = User", output);
            Assert.Contains("UserPart (Orchard.Users.Models.UserPart)", output);
            Assert.Contains("UserName = admin", output);
        }

        [Fact, Integration]
        public void ContentItemsShouldContainPropertiesFromContentParts() {
            var table = _powerShell.ExecuteTable(
                "Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Items\\User | ft UserName, UserPart");

            var userRow = table.Rows.Single(x => x[0] == "admin");
            Assert.Equal("Orchard.Users.Models.UserPart", userRow[1]);
        }
    }
}