namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class GetContentItemTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public GetContentItemTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldGetAllContentItems()
        {
            this.powerShell.Session.ProcessInput("Get-ContentItem");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count > 0);
        }

        [Fact, Integration]
        public void ShouldFormatContentItemsInGrid()
        {
            this.powerShell.Session.ProcessInput("Get-ContentItem");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal("Id", table.Header[0]);
            Assert.Equal("ContentType", table.Header[1]);
            Assert.Equal("Title", table.Header[2]);
        }

        [Fact, Integration]
        public void ShouldGetContentItemById()
        {
            this.powerShell.Session.ProcessInput("Get-ContentItem 1");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal("1", table.Rows.Single()[0]);
            Assert.Equal("Site", table.Rows.Single()[1]);
            Assert.Empty(table.Rows.Single()[2]);
        }

        [Fact, Integration]
        public void ShouldGetContentItemByContentType()
        {
            this.powerShell.Session.ProcessInput("Get-ContentItem -ContentType Layer");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count > 0);
            Assert.All(table.Rows, row => Assert.Equal("Layer", row[1]));
        }

        [Theory, Integration]
        [InlineData("Get-ContentItem 1 -VersionOptions Latest")]
        [InlineData("Get-ContentItem 1 -VersionOptions Published")]
        [InlineData("Get-ContentItem 1 -VersionOptions Draft")]
        [InlineData("Get-ContentItem 1 -VersionOptions DraftRequired")]
        [InlineData("Get-ContentItem 1 -VersionOptions AllVersions")]
        public void ShouldGetContentItemWithVersionOptions(string command)
        {
            this.powerShell.Session.ProcessInput(command);

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.True(table.Rows.Count > 0);
            Assert.All(table.Rows, row => Assert.Equal("Site", row[1]));
        }

        [Fact, Integration]
        public void ShouldGetContentItemWithDraftVersionOptions()
        {
            // NOTE:
            // First call Get-ContentItem with DraftRequired version options to make sure a draft exists, otherwise
            // Get-ContentItem with Draft version options may not return anything.
            
            var commands = new[]
            {
                "Get-ContentItem 1 -VersionOptions DraftRequired",
                "Get-ContentItem 1 -VersionOptions Draft",
            };
            
            foreach (var command in commands)
            {
                this.powerShell.ConsoleConnection.Reset();
                this.powerShell.Session.ProcessInput(command);

                string output = this.powerShell.ConsoleConnection.Output.ToString();
                this.powerShell.ConsoleConnection.AssertNoErrors();

                var table = PsTable.Parse(output);
                Assert.True(table.Rows.Count > 0);
                Assert.All(table.Rows, row => Assert.Equal("Site", row[1]));
            }
        }

        [Fact, Integration]
        public void ShouldGetContentItemByVersionNumber()
        {
            this.powerShell.Session.ProcessInput("Get-ContentItem -Id 1 -Version 1 | Format-Table Id, Version");

            string output = this.powerShell.ConsoleConnection.Output.ToString();
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var table = PsTable.Parse(output);
            Assert.Equal("1", table.Rows.Single()[0]);
            Assert.Equal("1", table.Rows.Single()[1]);
        }
    }
}