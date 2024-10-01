using System;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class RemoveContentItemTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public RemoveContentItemTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldRemoveContentItemById() {
            int id = CreateContentItem();
            _powerShell.Execute("Remove-ContentItem " + id + " -Verbose");

            Assert.Equal(
                "Performing the operation \"Remove\" on target \"Content Item: " + id +
                ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldDestroyContentItemById() {
            int id = CreateContentItem();
            _powerShell.Execute("Remove-ContentItem " + id + " -Destroy -Verbose");

            Assert.Equal(
                "Performing the operation \"Destroy\" on target \"Content Item: " + id +
                ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldRemoveContentItemByObject() {
            int id = CreateContentItem();
            _powerShell.Execute("Get-ContentItem -Id " + id + " | Remove-ContentItem -Verbose");

            Assert.Equal(
                "Performing the operation \"Remove\" on target \"Content Item: " + id +
                ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldDestroyContentItemByObject() {
            int id = CreateContentItem();
            _powerShell.Execute("Get-ContentItem -Id " + id + " | Remove-ContentItem -Destroy -Verbose");

            Assert.Equal(
                "Performing the operation \"Destroy\" on target \"Content Item: " + id +
                ", Version: 1, Tenant: Default\".",
                _powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            AssertContentItemDoesNotExist(id);
        }

        private int CreateContentItem() {
            _powerShell.Execute("$item = New-ContentItem Page -Published");
            var output = _powerShell.Execute("$item.Id");
            _powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }

        private void AssertContentItemDoesNotExist(int id) {
            _powerShell.ConsoleConnection.Reset();
            Assert.Empty(_powerShell.Execute("Get-ContentItem " + id));
        }
    }
}