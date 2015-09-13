using System;
using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class EditContentPartDefinitionTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public EditContentPartDefinitionTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldUpdateDescription() {
            string newDescription = Guid.NewGuid().ToString("N");
            _powerShell.Execute("Edit-ContentPartDefinition CommonPart -Description '" + newDescription + "'");
            Assert.Equal(newDescription, GetDescription("CommonPart"));
        }

        [Fact, Integration]
        public void ShouldUpdateAttachableFlag() {
            // Repeat test two times - first set/unset flag, then unset/se flag
            for (int i = 0; i < 2; i++) {
                if (IsAttachable("CommentPart")) {
                    _powerShell.Execute("Edit-ContentPartDefinition CommentPart -Attachable $false");
                    Assert.False(IsAttachable("CommentPart"));
                }
                else {
                    _powerShell.Execute("Edit-ContentPartDefinition CommentPart -Attachable $true");
                    Assert.True(IsAttachable("CommentPart"));
                }
            }
        }

        [Fact, Integration]
        public void ShouldUpdateCustomSetting() {
            var settingName = Guid.NewGuid().ToString("N");

            _powerShell.Execute("Edit-ContentPartDefinition CommonPart -" + settingName + " value");
            Assert.Equal("value", GetSettingValue("CommonPart", settingName));

            _powerShell.Execute("Edit-ContentPartDefinition CommonPart -" + settingName + " 'new value'");
            Assert.Equal("new value", GetSettingValue("CommonPart", settingName));
        }

        [Fact, Integration]
        public void ShouldDeleteCustomSetting() {
            var settingName = Guid.NewGuid().ToString("N");

            _powerShell.Execute("Edit-ContentPartDefinition CommonPart -" + settingName + " value");
            Assert.Equal("value", GetSettingValue("CommonPart", settingName));

            _powerShell.Execute("Edit-ContentPartDefinition CommonPart -" + settingName + " $null");
            Assert.Null(GetSettingValue("CommonPart", settingName));
        }

        [Fact, Integration]
        public void ShouldUpdateContentPartDefinitionObject() {
            string newDescription = Guid.NewGuid().ToString("N");

            _powerShell.Execute(
                "Get-ContentPartDefinition CommonPart | Edit-ContentPartDefinition -Description '" + newDescription +
                "'");

            Assert.Equal(newDescription, GetDescription("CommonPart"));
        }

        private string GetDescription(string contentPart) {
            var table = _powerShell.ExecuteTable("Get-ContentPartDefinition " + contentPart);
            _powerShell.ConsoleConnection.Reset();

            return table[0, "Description"];
        }

        private bool IsAttachable(string contentPart) {
            var table = _powerShell.ExecuteTable("Get-ContentPartDefinition " + contentPart);
            _powerShell.ConsoleConnection.Reset();

            return table[0, "Attachable"].Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private string GetSettingValue(string contentType, string settingName) {
            var table = _powerShell.ExecuteTable("(Get-ContentPartDefinition " + contentType + ").Settings");
            _powerShell.ConsoleConnection.Reset();

            var row = table.Rows.FirstOrDefault(r => r[0] == "ContentPartSettings." + settingName);
            if (row != null) {
                return row[1];
            }

            return null;
        }
    }
}