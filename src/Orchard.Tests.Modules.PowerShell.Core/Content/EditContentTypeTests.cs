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
            this.powerShell.Session.ProcessInput("Edit-ContentType Foo -DisplayName '" + newDisplayName + "'");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            this.powerShell.Session.ProcessInput("(Get-ContentType Foo).DisplayName");
            Assert.Equal(newDisplayName, this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }

        [Fact, Integration]
        public void ShouldUpdateStereotype()
        {
            this.EnsureContentTypeExists("Foo");

            string newStereotype = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("Edit-ContentType Foo -Stereotype " + newStereotype);
            this.powerShell.ConsoleConnection.AssertNoErrors();
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
                    this.powerShell.Session.ProcessInput("Edit-ContentType Foo -" + switchName + ":$false");
                    this.powerShell.ConsoleConnection.AssertNoErrors();
                    Assert.Equal("False", this.GetSettingValue("Foo", settingName));
                }
                else
                {
                    this.powerShell.Session.ProcessInput("Edit-ContentType Foo -" + switchName);
                    this.powerShell.ConsoleConnection.AssertNoErrors();
                    Assert.Equal("True", this.GetSettingValue("Foo", settingName));
                }
            }
        }

        [Fact, Integration]
        public void ShouldUpdateCustomSetting()
        {
            this.EnsureContentTypeExists("Foo");

            var settingName = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("Edit-ContentType Foo -Settings @{'" + settingName + "'='value'}");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Equal("value", this.GetSettingValue("Foo", settingName));

            this.powerShell.Session.ProcessInput("Edit-ContentType Foo -Settings @{'" + settingName + "'='new value'}");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Equal("new value", this.GetSettingValue("Foo", settingName));
        }

        [Fact, Integration]
        public void ShouldDeleteCustomSetting()
        {
            this.EnsureContentTypeExists("Foo");

            var settingName = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("Edit-ContentType Foo -Settings @{'" + settingName + "'='value'}");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Equal("value", this.GetSettingValue("Foo", settingName));
            
            this.powerShell.Session.ProcessInput("Edit-ContentType Foo -Settings @{'" + settingName + "'=$null}");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            Assert.Null(this.GetSettingValue("Foo", settingName));
        }

        [Fact, Integration]
        public void ShouldUpdateContentTypeObject()
        {
            this.EnsureContentTypeExists("Foo");

            string newDisplayName = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("Get-ContentType Foo | Edit-ContentType -DisplayName '" + newDisplayName + "'");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            this.powerShell.Session.ProcessInput("(Get-ContentType Foo).DisplayName");
            Assert.Equal(newDisplayName, this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }

        private void EnsureContentTypeExists(string name)
        {
            this.powerShell.Session.ProcessInput("Get-ContentType " + name);
            string output = this.powerShell.ConsoleConnection.Output.ToString();
            
            if (string.IsNullOrEmpty(output))
            {
                this.powerShell.Session.ProcessInput("New-ContentType " + name);
                this.powerShell.ConsoleConnection.Reset();
                this.powerShell.Session.ProcessInput("Get-ContentType " + name);
                Assert.NotEmpty(this.powerShell.ConsoleConnection.Output.ToString());
            }

            this.powerShell.ConsoleConnection.Reset();
        }

        private string GetSettingValue(string contentType, string settingName)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("(Get-ContentType " + contentType + ").Settings");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
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