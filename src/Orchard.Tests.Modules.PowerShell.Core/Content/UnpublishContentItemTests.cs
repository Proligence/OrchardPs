using System;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class UnpublishContentItemTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public UnpublishContentItemTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldUnpublishContentItemById() {
            int id = CreateContentItem();
            _powerShell.Execute("Unpublish-ContentItem " + id + " -Verbose");

            Assert.Equal(
                "Performing the operation \"Unpublish\" on target \"Content Item: " + id +
                ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemNotPublished(id);
        }

        [Fact, Integration]
        public void ShouldUnpublishContentItemByObject() {
            int id = CreateContentItem();
            _powerShell.Execute("Get-ContentItem -Id " + id + " -VersionOptions Latest | Unpublish-ContentItem -Verbose");

            Assert.Equal(
                "Performing the operation \"Unpublish\" on target \"Content Item: " + id +
                ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemNotPublished(id);
        }

        private int CreateContentItem() {
            _powerShell.Execute("$item = New-ContentItem Page -Published");
            var output = _powerShell.Execute("$item.Id");
            _powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }

        private void AssertContentItemNotPublished(int id) {
            _powerShell.ConsoleConnection.Reset();
            var output =
                _powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Latest).VersionRecord.Published");
            Assert.Equal("False", output);
        }
    }
}