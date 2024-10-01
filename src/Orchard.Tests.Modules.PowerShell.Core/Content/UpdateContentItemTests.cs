using System;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class UpdateContentItemTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public UpdateContentItemTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldUpdateContentItem() {
            _powerShell.Execute("$item = New-ContentItem Page -Published");
            var title = Guid.NewGuid().ToString("N");
            _powerShell.Execute("$item.TitlePart.Title = '" + title + "'");
            int id = Convert.ToInt32(_powerShell.Execute("$item.Id"));
            _powerShell.ConsoleConnection.Reset();
            _powerShell.Execute("Update-ContentItem $item");

            Assert.Equal(title, _powerShell.Execute("(Get-ContentItem -Id " + id + ").Title"));
        }

        [Fact, Integration]
        public void ShouldUpdateLatestVersionOfContentItem() {
            _powerShell.Execute("$item = New-ContentItem Page -Published");
            var title = Guid.NewGuid().ToString("N");
            _powerShell.Execute("$item.TitlePart.Title = '" + title + "'");
            int id = Convert.ToInt32(_powerShell.Execute("$item.Id"));
            _powerShell.ConsoleConnection.Reset();
            _powerShell.Execute("Update-ContentItem $item -VersionOptions Latest -Verbose");

            string verbose = _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim();
            Assert.Equal(
                "Performing the operation \"Update Latest\" on target \"Content Item: " + id + ", Tenant: Default\".",
                verbose);

            _powerShell.ConsoleConnection.Reset();
            Assert.Equal(title, _powerShell.Execute("(Get-ContentItem -Id " + id + ").Title"));
        }
    }
}