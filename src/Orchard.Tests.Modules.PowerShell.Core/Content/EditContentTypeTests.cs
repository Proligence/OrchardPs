namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class EditContentTypeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public EditContentTypeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldUpdateDisplayName()
        {
            this.EnsureContentTypeExists("Foo");
            string newDisplayName = Guid.NewGuid().ToString("N");
            this.powerShell.Execute("Edit-ContentType Foo -DisplayName '" + newDisplayName + "'");
            Assert.Equal(newDisplayName, this.powerShell.Execute("(Get-ContentType Foo).DisplayName"));
        }

        [Fact, Integration]
        public void ShouldUpdateStereotype()
        {
            this.EnsureContentTypeExists("Foo");
            string newStereotype = Guid.NewGuid().ToString("N");
            this.powerShell.Execute("Edit-ContentType Foo -Stereotype " + newStereotype);
            Assert.Equal(newStereotype, this.GetSettingValue("Foo", "Stereotype"));
        }

        [Theory, Integration]
        [InlineData("Creatable", "ContentTypeSettings.Creatable")]
        [InlineData("Listable", "ContentTypeSettings.Listable")]
        [InlineData("Draftable", "ContentTypeSettings.Draftable")]
        [InlineData("Securable", "ContentTypeSettings.Securable")]
        public void ShouldSetAndClearStandardSettingFlags(string switchName, string settingName)
        {
            this.EnsureContentTypeExists("Foo");

            // Repeat test two times - first set/unset flag, then unset/se flag
            for (int i = 0; i < 2; i++)
            {
                if (this.GetSettingValue("Foo", settingName) == "True")
                {
                    this.powerShell.Execute("Edit-ContentType Foo -" + switchName + ":$false");
                    Assert.Equal("False", this.GetSettingValue("Foo", settingName));
                }
                else
                {
                    this.powerShell.Execute("Edit-ContentType Foo -" + switchName);
                    Assert.Equal("True", this.GetSettingValue("Foo", settingName));
                }
            }
        }

        [Fact, Integration]
        public void ShouldUpdateCustomSetting()
        {
            this.EnsureContentTypeExists("Foo");
            var settingName = Guid.NewGuid().ToString("N");

            this.powerShell.Execute("Edit-ContentType Foo -Settings @{'" + settingName + "'='value'}");
            Assert.Equal("value", this.GetSettingValue("Foo", settingName));

            this.powerShell.Execute("Edit-ContentType Foo -Settings @{'" + settingName + "'='new value'}");
            Assert.Equal("new value", this.GetSettingValue("Foo", settingName));
        }

        [Fact, Integration]
        public void ShouldDeleteCustomSetting()
        {
            this.EnsureContentTypeExists("Foo");
            var settingName = Guid.NewGuid().ToString("N");

            this.powerShell.Execute("Edit-ContentType Foo -Settings @{'" + settingName + "'='value'}");
            Assert.Equal("value", this.GetSettingValue("Foo", settingName));
            
            this.powerShell.Execute("Edit-ContentType Foo -Settings @{'" + settingName + "'=$null}");
            Assert.Null(this.GetSettingValue("Foo", settingName));
        }

        [Fact, Integration]
        public void ShouldUpdateContentTypeObject()
        {
            this.EnsureContentTypeExists("Foo");
            string newDisplayName = Guid.NewGuid().ToString("N");
            this.powerShell.Execute("Get-ContentType Foo | Edit-ContentType -DisplayName '" + newDisplayName + "'");
            Assert.Equal(newDisplayName, this.powerShell.Execute("(Get-ContentType Foo).DisplayName"));
        }

        private void EnsureContentTypeExists(string name)
        {
            var output = this.powerShell.Execute("Get-ContentType " + name);
            if (string.IsNullOrEmpty(output))
            {
                this.powerShell.Execute("New-ContentType " + name);
                this.powerShell.ConsoleConnection.Reset();
                Assert.NotEmpty(this.powerShell.Execute("Get-ContentType " + name));
            }

            this.powerShell.ConsoleConnection.Reset();
        }

        private string GetSettingValue(string contentType, string settingName)
        {
            var table = this.powerShell.ExecuteTable("(Get-ContentType " + contentType + ").Settings");
            this.powerShell.ConsoleConnection.Reset();

            var row = table.Rows.FirstOrDefault(r => r[0] == settingName);
            if (row != null)
            {
                return row[1];
            }

            return null;
        }
    }
}