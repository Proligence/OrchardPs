using System;
using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class CopyContentItemTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public CopyContentItemTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCopyContentItemById() {
            int id = CreateContentItem();
            var table = _powerShell.ExecuteTable("Copy-ContentItem " + id + " -Verbose");

            Assert.Equal(
                "Performing the operation \"Copy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            var newId = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(newId > id);
        }

        [Fact, Integration]
        public void ShouldCopyContentItemByObject() {
            int id = CreateContentItem();

            var table = _powerShell.ExecuteTable(
                "Get-ContentItem -Id " + id + " -VersionOptions Latest | Copy-ContentItem -Verbose");

            Assert.Equal(
                "Performing the operation \"Copy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            var newId = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(newId > id);
        }

        private int CreateContentItem() {
            _powerShell.Execute("$item = New-ContentItem Page -Published");
            var output = _powerShell.Execute("$item.Id");
            _powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }
    }
}