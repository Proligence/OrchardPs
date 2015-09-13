using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class NewContentTypeTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public NewContentTypeTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCreateContentType() {
            EnsureContentTypeDoesNotExist("Foo");
            _powerShell.Execute("New-ContentType Foo");
            var table = _powerShell.ExecuteTable("Get-ContentType Foo");
            Assert.Equal("Foo", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldCreateContentTypeWithCustomDisplayName() {
            EnsureContentTypeDoesNotExist("Foo");
            _powerShell.Execute("New-ContentType Foo -DisplayName 'My Foo'");
            var table = _powerShell.ExecuteTable("Get-ContentType Foo");
            Assert.Equal("Foo", table.Rows.Single()[0]);
            Assert.Equal("My Foo", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateNewContentTypeWithCustomStereotype() {
            EnsureContentTypeDoesNotExist("Foo");
            _powerShell.Execute("New-ContentType Foo -Stereotype Bar");
            var output = _powerShell.Execute("(Get-ContentType Foo).Settings['Stereotype']");
            Assert.Equal("Bar", output);
        }

        [Theory, Integration]
        [InlineData("Creatable", "ContentTypeSettings.Creatable")]
        [InlineData("Listable", "ContentTypeSettings.Listable")]
        [InlineData("Draftable", "ContentTypeSettings.Draftable")]
        [InlineData("Securable", "ContentTypeSettings.Securable")]
        public void ShouldCreateContentTypeWithStandardSettings(string switchName, string settingName) {
            EnsureContentTypeDoesNotExist("Foo");
            _powerShell.Execute("New-ContentType Foo -" + switchName);
            var output = _powerShell.Execute("(Get-ContentType Foo).Settings['" + settingName + "']");
            Assert.Equal("True", output);
        }

        [Fact, Integration]
        public void ShouldCreateNewContentTypeWithContentParts() {
            EnsureContentTypeDoesNotExist("Foo");
            _powerShell.Execute("New-ContentType Foo -Parts ('CommonPart', 'TitlePart')");
            var table = _powerShell.ExecuteTable("Get-ContentType Foo");
            Assert.Equal("Foo", table.Rows.Single()[0]);
            Assert.Equal("CommonPart, TitlePart", table.Rows.Single()[2]);
        }

        [Fact, Integration]
        public void ShouldCreateContentTypeWithCustomSettings() {
            EnsureContentTypeDoesNotExist("Foo");
            _powerShell.Execute("New-ContentType Foo -Settings @{'Bar'='Bar value'; 'Baz'='Baz value'}");
            var table = _powerShell.ExecuteTable("(Get-ContentType Foo).Settings");
            Assert.Equal(2, table.Rows.Count);
            Assert.Equal("Bar", table[0, "Key"]);
            Assert.Equal("Bar value", table[0, "Value"]);
            Assert.Equal("Baz", table[1, "Key"]);
            Assert.Equal("Baz value", table[1, "Value"]);
        }

        private void EnsureContentTypeDoesNotExist(string name) {
            var output = _powerShell.Execute("Get-ContentType " + name);
            if (!string.IsNullOrEmpty(output)) {
                _powerShell.Execute("Remove-ContentType " + name);
                _powerShell.ConsoleConnection.Reset();
                Assert.Empty(_powerShell.Execute("Get-ContentType " + name));
            }
        }
    }
}