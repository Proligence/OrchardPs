namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class RestoreContentItemTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public RestoreContentItemTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldRestoreContentItemById()
        {
            int id = this.CreateContentItem();
            this.AssertContentItemTitle(id, "baz");
            this.powerShell.Execute("Restore-ContentItem " + id + " 2 -Verbose");
            
            Assert.Equal(
                "Performing the operation \"Restore\" on target \"Content Item: " + id + ", Version: 2, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemTitle(id, "bar");
        }

        [Fact, Integration]
        public void ShouldRestoreContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.AssertContentItemTitle(id, "baz");
            
            this.powerShell.Execute(
                "Get-ContentItem -Id " + id + " -VersionOptions Latest | Restore-ContentItem -Version 2 -Verbose");

            Assert.Equal(
                "Performing the operation \"Restore\" on target \"Content Item: " + id + ", Version: 2, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemTitle(id, "bar");
        }

        [Fact, Integration]
        public void ShouldFailWhenVersionNumberNotSpecified()
        {
            int id = this.CreateContentItem();
            this.AssertContentItemTitle(id, "baz");
            this.powerShell.Session.ProcessInput("Restore-ContentItem " + id + " -Verbose");

            var expected = "The VersionOptions or Version parameter must be specified.";
            Assert.True(this.powerShell.ConsoleConnection.ErrorOutput.ToString().Contains(expected));
            this.AssertContentItemTitle(id, "baz");
        }

        private int CreateContentItem()
        {
            this.powerShell.Execute("$item = New-ContentItem Page -Draft");
            int id = Convert.ToInt32(this.powerShell.Execute("$item.Id"));

            // Version 1
            this.powerShell.Execute("$item.TitlePart.Title = 'foo'");
            this.powerShell.Execute("Update-ContentItem $item -VersionOptions DraftRequired");
            this.powerShell.Execute("Publish-ContentItem $item -VersionOptions Latest");

            // Version 2
            this.powerShell.Execute("$item.TitlePart.Title = 'bar'");
            this.powerShell.Execute("Update-ContentItem $item -VersionOptions DraftRequired");
            this.powerShell.Execute("Publish-ContentItem $item -VersionOptions Latest");

            // Version 3
            this.powerShell.Execute("$item.TitlePart.Title = 'baz'");
            this.powerShell.Execute("Update-ContentItem $item -VersionOptions DraftRequired");
            this.powerShell.Execute("Publish-ContentItem $item -VersionOptions Latest");
            
            this.powerShell.ConsoleConnection.Reset();

            return id;
        }

        private void AssertContentItemTitle(int id, string expectedTitle)
        {
            this.powerShell.ConsoleConnection.Reset();
            Assert.Equal(expectedTitle, this.powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Latest).Title"));
        }
    }
}