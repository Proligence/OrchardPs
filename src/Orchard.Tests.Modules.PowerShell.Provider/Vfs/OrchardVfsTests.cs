namespace Orchard.Tests.Modules.PowerShell.Provider.Vfs
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class OrchardVfsTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public OrchardVfsTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsRootShouldContainTenants()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTestHelper.ParseTable(output);
            Assert.Equal("Name", table[0][0]);
            Assert.Equal(1, table.Count(x => x[0] == "Tenants"));
        }

        [Fact, Integration]
        public void VfsShouldContainDefaultTenant()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTestHelper.ParseTable(output);
            Assert.Equal("Name", table[0][0]);
            Assert.Equal("State", table[0][1]);
            Assert.Equal(1, table.Count(x => x[0] == "Default"));
            Assert.Equal("Running", table.First(x => x[0] == "Default")[1]);
        }
    }
}