namespace Orchard.Tests.Modules.PowerShell.Core.Modules
{
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class EnableOrchardFeatureTests : IClassFixture<PowerShellFixture>
    {
        private const string TestFeatureId = "Orchard.Blogs";
        private readonly PowerShellFixture powerShell;

        public EnableOrchardFeatureTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact(Skip = "This needs to be fixed."), Integration]
        public void ShouldEnableFeatureByName()
        {
            // First make sure that the feature is disabled
            this.powerShell.Execute("Invoke-OrchardCommand feature disable " + TestFeatureId);
            Assert.Equal("False", this.GetOrchardFeature(TestFeatureId)[0, "Enabled"]);
            
            this.powerShell.Execute("Enable-OrchardFeature " + TestFeatureId);
            Assert.Equal("True", this.GetOrchardFeature(TestFeatureId)[0, "Enabled"]);
        }

        private PsTable GetOrchardFeature(string featureId)
        {
            var table = this.powerShell.ExecuteTable("Get-OrchardFeature " + featureId);
            this.powerShell.ConsoleConnection.Reset();

            return table;
        }
    }
}