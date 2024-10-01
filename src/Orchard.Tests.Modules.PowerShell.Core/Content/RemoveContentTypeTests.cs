using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Content {
    [Collection("PowerShell")]
    public class RemoveContentTypeTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public RemoveContentTypeTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldRemoveContentTypeByName() {
            EnsureContentTypeExists("Foo");
            _powerShell.Execute("Remove-ContentType Foo");
            Assert.Empty(_powerShell.Execute("Get-ContentType Foo"));
        }

        [Fact, Integration]
        public void ShouldRemoveContentTypeByObject() {
            EnsureContentTypeExists("Foo");
            _powerShell.Execute("Get-ContentType Foo | Remove-ContentType");
            Assert.Empty(_powerShell.Execute("Get-ContentType Foo"));
        }

        private void EnsureContentTypeExists(string name) {
            _powerShell.ConsoleConnection.Reset();
            var output = _powerShell.Execute("Get-ContentType " + name);
            if (string.IsNullOrEmpty(output)) {
                _powerShell.Execute("New-ContentType " + name);
                _powerShell.ConsoleConnection.Reset();
                Assert.NotEmpty(_powerShell.Execute("Get-ContentType " + name));
            }

            _powerShell.ConsoleConnection.Reset();
        }
    }
}