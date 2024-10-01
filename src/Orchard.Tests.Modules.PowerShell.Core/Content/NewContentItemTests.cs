using System;
using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class NewContentItemTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public NewContentItemTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCreateContentItem() {
            var table = _powerShell.ExecuteTable("New-ContentItem Page");
            Assert.Equal("0", table.Rows.Single()[0]);
            Assert.Equal("Page", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateContentItemFromContentTypeObject() {
            var table = _powerShell.ExecuteTable("Get-ContentType Page | New-ContentItem");
            Assert.Equal("0", table.Rows.Single()[0]);
            Assert.Equal("Page", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateDraftContentItem() {
            var table = _powerShell.ExecuteTable("New-ContentItem Page -Draft");
            int id = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(id > 0);
            Assert.Equal("Page", table.Rows.Single()[1]);

            _powerShell.ConsoleConnection.Reset();
            var output = _powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Draft).VersionRecord.Published");
            Assert.Equal("False", output);
        }

        [Fact, Integration]
        public void ShouldCreatePublishedContentItem() {
            var table = _powerShell.ExecuteTable("New-ContentItem Page -Published");
            int id = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(id > 0);
            Assert.Equal("Page", table.Rows.Single()[1]);

            _powerShell.ConsoleConnection.Reset();
            var output = _powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Published).VersionRecord.Published");
            Assert.Equal("True", output);
        }
    }
}