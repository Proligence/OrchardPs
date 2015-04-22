namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class ContentVfsNodes : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public ContentVfsNodes(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainContentParts()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Parts");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Attachable", table.Header[1]);
            Assert.Equal("Fields", table.Header[2]);
            Assert.Equal("Description", table.Header[3]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "CommonPart"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "ContainerPart"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "TitlePart"));
        }

        [Fact, Integration]
        public void VfsTenantShouldContainContentTypes()
        {
            this.powerShell.Session.ProcessInput("Get-ChildItem Orchard:\\Tenants\\Default\\Content\\Types");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("DisplayName", table.Header[1]);
            Assert.Equal("Parts", table.Header[2]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Layer"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "Site"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "User"));
        }
    }
}