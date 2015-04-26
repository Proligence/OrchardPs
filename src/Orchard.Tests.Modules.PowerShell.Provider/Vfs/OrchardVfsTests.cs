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

            var table = PsTable.Parse(output);
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Tenants"));
        }

        [Fact, Integration]
        public void VfsShouldContainDefaultTenant()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("State", table.Header[1]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Default"));
            Assert.Equal("Running", table.Rows.First(x => x[0] == "Default")[1]);
        }

        [Fact, Integration]
        public void VfsShouldListTenantContents()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());
            Assert.NotEmpty(this.powerShell.ConsoleConnection.Output.ToString());
        }

        [Fact, Integration]
        public void VfsShouldContainRootDefaultTenant()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(output);
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "$"));
        }

        [Fact, Integration]
        public void VfsShouldListRootDefaultTenantContents()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\$");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());
            Assert.NotEmpty(this.powerShell.ConsoleConnection.Output.ToString());
        }
    }
}