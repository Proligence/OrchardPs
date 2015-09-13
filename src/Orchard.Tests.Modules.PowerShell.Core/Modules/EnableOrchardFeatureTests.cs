using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Modules {
    [Collection("PowerShell")]
    public class EnableOrchardFeatureTests : IClassFixture<PowerShellFixture> {
        private const string TestFeatureId = "Orchard.Blogs";
        private readonly PowerShellFixture _powerShell;

        public EnableOrchardFeatureTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact(Skip = "This needs to be fixed."), Integration]
        public void ShouldEnableFeatureByName() {
            // First make sure that the feature is disabled
            _powerShell.Execute("Invoke-OrchardCommand feature disable " + TestFeatureId);
            Assert.Equal("False", GetOrchardFeature(TestFeatureId)[0, "Enabled"]);

            _powerShell.Execute("Enable-OrchardFeature " + TestFeatureId);
            Assert.Equal("True", GetOrchardFeature(TestFeatureId)[0, "Enabled"]);
        }

        private PsTable GetOrchardFeature(string featureId) {
            var table = _powerShell.ExecuteTable("Get-OrchardFeature " + featureId);
            _powerShell.ConsoleConnection.Reset();

            return table;
        }
    }
}