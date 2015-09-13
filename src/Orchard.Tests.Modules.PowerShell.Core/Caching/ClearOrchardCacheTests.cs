using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Caching {
    [Collection("PowerShell")]
    public class ClearOrchardCacheTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public ClearOrchardCacheTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldClearOrchardCache() {
            var output = _powerShell.Execute("Clear-OrchardCache");
            Assert.Empty(output);
        }

        [Fact, Integration]
        public void ClearOrchardCacheShouldSupportWhatIf() {
            var output = _powerShell.Execute("Clear-OrchardCache -WhatIf");
            Assert.Equal("What if: Performing the operation \"Clear\" on target \"OrchardCache\".", output);
        }
    }
}