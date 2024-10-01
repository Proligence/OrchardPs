using System;
using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class EditContentTypeTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public EditContentTypeTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldUpdateDisplayName() {
            EnsureContentTypeExists("Foo");
            string newDisplayName = Guid.NewGuid().ToString("N");
            _powerShell.Execute("Edit-ContentType Foo -DisplayName '" + newDisplayName + "'");
            Assert.Equal(newDisplayName, _powerShell.Execute("(Get-ContentType Foo).DisplayName"));
        }

        [Fact, Integration]
        public void ShouldUpdateStereotype() {
            EnsureContentTypeExists("Foo");
            string newStereotype = Guid.NewGuid().ToString("N");
            _powerShell.Execute("Edit-ContentType Foo -Stereotype " + newStereotype);
            Assert.Equal(newStereotype, GetSettingValue("Foo", "Stereotype"));
        }

        [Theory, Integration]
        [InlineData("Creatable", "ContentTypeSettings.Creatable")]
        [InlineData("Listable", "ContentTypeSettings.Listable")]
        [InlineData("Draftable", "ContentTypeSettings.Draftable")]
        [InlineData("Securable", "ContentTypeSettings.Securable")]
        public void ShouldSetAndClearStandardSettingFlags(string switchName, string settingName) {
            EnsureContentTypeExists("Foo");

            // Repeat test two times - first set/unset flag, then unset/se flag
            for (int i = 0; i < 2; i++) {
                if (GetSettingValue("Foo", settingName) == "True") {
                    _powerShell.Execute("Edit-ContentType Foo -" + switchName + ":$false");
                    Assert.Equal("False", GetSettingValue("Foo", settingName));
                }
                else {
                    _powerShell.Execute("Edit-ContentType Foo -" + switchName);
                    Assert.Equal("True", GetSettingValue("Foo", settingName));
                }
            }
        }

        [Fact, Integration]
        public void ShouldUpdateCustomSetting() {
            EnsureContentTypeExists("Foo");
            var settingName = Guid.NewGuid().ToString("N");

            _powerShell.Execute("Edit-ContentType Foo -Settings @{'" + settingName + "'='value'}");
            Assert.Equal("value", GetSettingValue("Foo", settingName));

            _powerShell.Execute("Edit-ContentType Foo -Settings @{'" + settingName + "'='new value'}");
            Assert.Equal("new value", GetSettingValue("Foo", settingName));
        }

        [Fact, Integration]
        public void ShouldDeleteCustomSetting() {
            EnsureContentTypeExists("Foo");
            var settingName = Guid.NewGuid().ToString("N");

            _powerShell.Execute("Edit-ContentType Foo -Settings @{'" + settingName + "'='value'}");
            Assert.Equal("value", GetSettingValue("Foo", settingName));

            _powerShell.Execute("Edit-ContentType Foo -Settings @{'" + settingName + "'=$null}");
            Assert.Null(GetSettingValue("Foo", settingName));
        }

        [Fact, Integration]
        public void ShouldUpdateContentTypeObject() {
            EnsureContentTypeExists("Foo");
            string newDisplayName = Guid.NewGuid().ToString("N");
            _powerShell.Execute("Get-ContentType Foo | Edit-ContentType -DisplayName '" + newDisplayName + "'");
            Assert.Equal(newDisplayName, _powerShell.Execute("(Get-ContentType Foo).DisplayName"));
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

        private string GetSettingValue(string contentType, string settingName) {
            var table = _powerShell.ExecuteTable("(Get-ContentType " + contentType + ").Settings");
            _powerShell.ConsoleConnection.Reset();

            var row = table.Rows.FirstOrDefault(r => r[0] == settingName);
            if (row != null) {
                return row[1];
            }

            return null;
        }
    }
}