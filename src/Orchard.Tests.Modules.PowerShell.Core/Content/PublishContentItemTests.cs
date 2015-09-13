using System;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class PublishContentItemTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public PublishContentItemTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldPublishContentItemById() {
            int id = CreateContentItem();
            _powerShell.Execute("Publish-ContentItem " + id + " -Verbose");

            Assert.Equal(
                "Performing the operation \"Publish\" on target \"Content Item: " + id +
                ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemPublished(id);
        }

        [Fact, Integration]
        public void ShouldPublishContentItemByObject() {
            int id = CreateContentItem();
            _powerShell.Execute("Get-ContentItem -Id " + id + " -VersionOptions Latest | Publish-ContentItem -Verbose");

            Assert.Equal(
                "Performing the operation \"Publish\" on target \"Content Item: " + id +
                ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemPublished(id);
        }

        private int CreateContentItem() {
            _powerShell.Execute("$item = New-ContentItem Page -Draft");
            var output = _powerShell.Execute("$item.Id");
            _powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }

        private void AssertContentItemPublished(int id) {
            _powerShell.ConsoleConnection.Reset();
            var output = _powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Latest).VersionRecord.Published");
            Assert.Equal("True", output);
        }
    }
}