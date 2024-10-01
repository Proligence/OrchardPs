using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class AlterContentPartFieldCmdletBaseTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public AlterContentPartFieldCmdletBaseTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentFieldByName() {
            // Repeat the test two times to add/remove content field, and then to remove/add content field
            for (int i = 0; i < 2; i++) {
                if (HasContentField("TitlePart", "BooleanField")) {
                    _powerShell.Execute("Remove-ContentField TitlePart BooleanField");
                    Assert.False(HasContentField("TitlePart", "BooleanField"));
                }
                else {
                    _powerShell.Execute("Add-ContentField TitlePart BooleanField");
                    Assert.True(HasContentField("TitlePart", "BooleanField"));
                }
            }
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentFieldByContentPartObject() {
            // Repeat the test two times to add/remove content field, and then to remove/add content field
            for (int i = 0; i < 2; i++) {
                if (HasContentField("TitlePart", "BooleanField")) {
                    _powerShell.Execute(
                        "Get-ContentPartDefinition TitlePart | Remove-ContentField -ContentField BooleanField");
                    Assert.False(HasContentField("TitlePart", "BooleanField"));
                }
                else {
                    _powerShell.Execute(
                        "Get-ContentPartDefinition TitlePart | Add-ContentField -ContentField BooleanField");
                    Assert.True(HasContentField("TitlePart", "BooleanField"));
                }
            }
        }

        private bool HasContentField(string contentPart, string contentField) {
            _powerShell.ConsoleConnection.Reset();
            var output = _powerShell.Execute("(Get-ContentPartDefinition " + contentPart + ").Fields");
            _powerShell.ConsoleConnection.Reset();

            if (!string.IsNullOrEmpty(output)) {
                var table = PsTable.Parse(output);
                var fields = table.Rows.Select(r => r[0]).ToArray();
                return fields.Any(p => p == contentField);
            }

            return false;
        }
    }
}