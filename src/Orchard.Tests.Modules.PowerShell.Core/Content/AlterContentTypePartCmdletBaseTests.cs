using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class AlterContentTypePartCmdletBaseTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public AlterContentTypePartCmdletBaseTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentPartByName() {
            EnsureContentTypeExists("Foo");

            // Repeat the test two times to add/remove content part, and then to remove/add content part
            for (int i = 0; i < 2; i++) {
                if (HasContentPart("Foo", "TitlePart")) {
                    _powerShell.Execute("Remove-ContentPart Foo TitlePart");
                    Assert.False(HasContentPart("Foo", "TitlePart"));
                }
                else {
                    _powerShell.Execute("Add-ContentPart Foo TitlePart");
                    Assert.True(HasContentPart("Foo", "TitlePart"));
                }
            }
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentPartByContentTypeObject() {
            EnsureContentTypeExists("Foo");

            // Repeat the test two times to add/remove content part, and then to remove/add content part
            for (int i = 0; i < 2; i++) {
                if (HasContentPart("Foo", "TitlePart")) {
                    _powerShell.Execute("Get-ContentType Foo | Remove-ContentPart -ContentPart TitlePart");
                    Assert.False(HasContentPart("Foo", "TitlePart"));
                }
                else {
                    _powerShell.Execute("Get-ContentType Foo | Add-ContentPart -ContentPart TitlePart");
                    Assert.True(HasContentPart("Foo", "TitlePart"));
                }
            }
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentPartByContentPartObject() {
            EnsureContentTypeExists("Foo");

            // Repeat the test two times to add/remove content part, and then to remove/add content part
            for (int i = 0; i < 2; i++) {
                if (HasContentPart("Foo", "TitlePart")) {
                    _powerShell.Execute("Get-ContentPartDefinition TitlePart | Remove-ContentPart -ContentType Foo");
                    Assert.False(HasContentPart("Foo", "TitlePart"));
                }
                else {
                    _powerShell.Execute("Get-ContentPartDefinition TitlePart | Add-ContentPart -ContentType Foo");
                    Assert.True(HasContentPart("Foo", "TitlePart"));
                }
            }
        }

        private void EnsureContentTypeExists(string name) {
            var output = _powerShell.Execute("Get-ContentType " + name);
            if (string.IsNullOrEmpty(output)) {
                _powerShell.Execute("New-ContentType " + name);
                _powerShell.ConsoleConnection.Reset();
                Assert.NotEmpty(_powerShell.Execute("Get-ContentType " + name));
            }

            _powerShell.ConsoleConnection.Reset();
        }

        private bool HasContentPart(string contentType, string contentPart) {
            var table = _powerShell.ExecuteTable("Get-ContentType " + contentType);
            var parts = table[0, "Parts"].Split(',').Select(str => str.Trim()).ToArray();
            _powerShell.ConsoleConnection.Reset();

            return parts.Any(p => p == contentPart);
        }
    }
}