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
            this.powerShell.Session.ProcessInput("Invoke-OrchardCommand feature disable " + TestFeatureId);
            Assert.Equal("False", this.GetOrchardFeature(TestFeatureId)[0, "Enabled"]);
            
            this.powerShell.Session.ProcessInput("Enable-OrchardFeature " + TestFeatureId);

            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());
            Assert.Equal("True", this.GetOrchardFeature(TestFeatureId)[0, "Enabled"]);
        }

        private PsTable GetOrchardFeature(string featureId)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-OrchardFeature " + featureId);
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            var table = PsTable.Parse(output);
            this.powerShell.ConsoleConnection.Reset();

            return table;
        }
    }
}