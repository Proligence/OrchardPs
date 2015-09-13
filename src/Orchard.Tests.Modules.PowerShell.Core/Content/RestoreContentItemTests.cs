using System;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class RestoreContentItemTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public RestoreContentItemTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldRestoreContentItemById() {
            int id = CreateContentItem();
            AssertContentItemTitle(id, "baz");
            _powerShell.Execute("Restore-ContentItem " + id + " 2 -Verbose");

            Assert.Equal(
                "Performing the operation \"Restore\" on target \"Content Item: " + id +
                ", Version: 2, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemTitle(id, "bar");
        }

        [Fact, Integration]
        public void ShouldRestoreContentItemByObject() {
            int id = CreateContentItem();
            AssertContentItemTitle(id, "baz");

            _powerShell.Execute(
                "Get-ContentItem -Id " + id + " -VersionOptions Latest | Restore-ContentItem -Version 2 -Verbose");

            Assert.Equal(
                "Performing the operation \"Restore\" on target \"Content Item: " + id +
                ", Version: 2, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemTitle(id, "bar");
        }

        [Fact, Integration]
        public void ShouldFailWhenVersionNumberNotSpecified() {
            int id = CreateContentItem();
            AssertContentItemTitle(id, "baz");
            _powerShell.Session.ProcessInput("Restore-ContentItem " + id + " -Verbose");

            var expected = "The VersionOptions or Version parameter must be specified.";
            Assert.True(_powerShell.ConsoleConnection.ErrorOutput.ToString().Contains(expected));
            AssertContentItemTitle(id, "baz");
        }

        private int CreateContentItem() {
            _powerShell.Execute("$item = New-ContentItem Page -Draft");
            int id = Convert.ToInt32(_powerShell.Execute("$item.Id"));

            // Version 1
            _powerShell.Execute("$item.TitlePart.Title = 'foo'");
            _powerShell.Execute("Update-ContentItem $item -VersionOptions DraftRequired");
            _powerShell.Execute("Publish-ContentItem $item -VersionOptions Latest");

            // Version 2
            _powerShell.Execute("$item.TitlePart.Title = 'bar'");
            _powerShell.Execute("Update-ContentItem $item -VersionOptions DraftRequired");
            _powerShell.Execute("Publish-ContentItem $item -VersionOptions Latest");

            // Version 3
            _powerShell.Execute("$item.TitlePart.Title = 'baz'");
            _powerShell.Execute("Update-ContentItem $item -VersionOptions DraftRequired");
            _powerShell.Execute("Publish-ContentItem $item -VersionOptions Latest");

            _powerShell.ConsoleConnection.Reset();

            return id;
        }

        private void AssertContentItemTitle(int id, string expectedTitle) {
            _powerShell.ConsoleConnection.Reset();

            Assert.Equal(
                expectedTitle,
                _powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Latest).Title"));
        }
    }
}