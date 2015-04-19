namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class NewContentTypeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public NewContentTypeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCreateContentType()
        {
            this.EnsureContentTypeDoesNotExist("Foo");

            this.powerShell.Session.ProcessInput("New-ContentType Foo");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("Get-ContentType Foo");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Foo", table.Rows.Single()[0]);
        }

        [Fact, Integration]
        public void ShouldCreateContentTypeWithCustomDisplayName()
        {
            this.EnsureContentTypeDoesNotExist("Foo");

            this.powerShell.Session.ProcessInput("New-ContentType Foo -DisplayName 'My Foo'");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("Get-ContentType Foo");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Foo", table.Rows.Single()[0]);
            Assert.Equal("My Foo", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateNewContentTypeWithCustomStereotype()
        {
            this.EnsureContentTypeDoesNotExist("Foo");

            this.powerShell.Session.ProcessInput("New-ContentType Foo -Stereotype Bar");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("(Get-ContentType Foo).Settings['Stereotype']");
            Assert.Equal("Bar", this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }

        [Theory, Integration]
        [InlineData("Creatable", "ContentTypeSettings.Creatable")]
        [InlineData("Listable", "ContentTypeSettings.Listable")]
        [InlineData("Draftable", "ContentTypeSettings.Draftable")]
        [InlineData("Securable", "ContentTypeSettings.Securable")]
        public void ShouldCreateContentTypeWithStandardSettings(string switchName, string settingName)
        {
            this.EnsureContentTypeDoesNotExist("Foo");

            this.powerShell.Session.ProcessInput("New-ContentType Foo -" + switchName);
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("(Get-ContentType Foo).Settings['" + settingName + "']");
            Assert.Equal("True", this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }

        [Fact, Integration]
        public void ShouldCreateNewContentTypeWithContentParts()
        {
            this.EnsureContentTypeDoesNotExist("Foo");

            this.powerShell.Session.ProcessInput("New-ContentType Foo -Parts ('CommonPart', 'TitlePart')");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("Get-ContentType Foo");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Foo", table.Rows.Single()[0]);
            Assert.Equal("CommonPart, TitlePart", table.Rows.Single()[2]);
        }

        [Fact, Integration]
        public void ShouldCreateContentTypeWithCustomSettings()
        {
            this.EnsureContentTypeDoesNotExist("Foo");

            this.powerShell.Session.ProcessInput("New-ContentType Foo -Settings @{'Bar'='Bar value'; 'Baz'='Baz value'}");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("(Get-ContentType Foo).Settings");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal(2, table.Rows.Count);
            Assert.Equal("Bar", table[0, "Key"]);
            Assert.Equal("Bar value", table[0, "Value"]);
            Assert.Equal("Baz", table[1, "Key"]);
            Assert.Equal("Baz value", table[1, "Value"]);
        }

        private void EnsureContentTypeDoesNotExist(string name)
        {
            this.powerShell.Session.ProcessInput("Get-ContentType " + name);
            string output = this.powerShell.ConsoleConnection.Output.ToString();
            
            if (!string.IsNullOrEmpty(output))
            {
                this.powerShell.Session.ProcessInput("Remove-ContentType " + name);
                this.powerShell.ConsoleConnection.Reset();
                this.powerShell.Session.ProcessInput("Get-ContentType " + name);
                Assert.Empty(this.powerShell.ConsoleConnection.Output.ToString());
            }
        }
    }
}