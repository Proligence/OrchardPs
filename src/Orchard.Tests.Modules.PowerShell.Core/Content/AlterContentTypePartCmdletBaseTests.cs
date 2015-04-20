namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class AlterContentTypePartCmdletBaseTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public AlterContentTypePartCmdletBaseTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentPartByName()
        {
            this.EnsureContentTypeExists("Foo");

            // Repeat the test two times to add/remove content part, and then to remove/add content part
            for (int i = 0; i < 2; i++)
            {
                if (this.HasContentPart("Foo", "TitlePart"))
                {
                    this.powerShell.Session.ProcessInput("Remove-ContentPart Foo TitlePart");
                    Assert.False(this.HasContentPart("Foo", "TitlePart"));
                }
                else
                {
                    this.powerShell.Session.ProcessInput("Add-ContentPart Foo TitlePart");
                    Assert.True(this.HasContentPart("Foo", "TitlePart"));
                }
            }
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentPartByContentTypeObject()
        {
            this.EnsureContentTypeExists("Foo");

            // Repeat the test two times to add/remove content part, and then to remove/add content part
            for (int i = 0; i < 2; i++)
            {
                if (this.HasContentPart("Foo", "TitlePart"))
                {
                    this.powerShell.Session.ProcessInput("Get-ContentType Foo | Remove-ContentPart -ContentPart TitlePart");
                    Assert.False(this.HasContentPart("Foo", "TitlePart"));
                }
                else
                {
                    this.powerShell.Session.ProcessInput("Get-ContentType Foo | Add-ContentPart -ContentPart TitlePart");
                    Assert.True(this.HasContentPart("Foo", "TitlePart"));
                }
            }
        }

        [Fact, Integration]
        public void ShouldAddAndRemoveContentPartByContentPartObject()
        {
            this.EnsureContentTypeExists("Foo");

            // Repeat the test two times to add/remove content part, and then to remove/add content part
            for (int i = 0; i < 2; i++)
            {
                if (this.HasContentPart("Foo", "TitlePart"))
                {
                    this.powerShell.Session.ProcessInput("Get-ContentPartDefinition TitlePart | Remove-ContentPart -ContentType Foo");
                    Assert.False(this.HasContentPart("Foo", "TitlePart"));
                }
                else
                {
                    this.powerShell.Session.ProcessInput("Get-ContentPartDefinition TitlePart | Add-ContentPart -ContentType Foo");
                    Assert.True(this.HasContentPart("Foo", "TitlePart"));
                }
            }
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

        private bool HasContentPart(string contentType, string contentPart)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-ContentType " + contentType);
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            var parts = table[0, "Parts"].Split(',').Select(str => str.Trim()).ToArray();
            this.powerShell.ConsoleConnection.Reset();

            return parts.Any(p => p == contentPart);
        }
    }
}