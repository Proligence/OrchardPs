namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class NewContentItemTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public NewContentItemTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCreateContentItem()
        {
            this.powerShell.Session.ProcessInput("New-ContentItem Page");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("0", table.Rows.Single()[0]);
            Assert.Equal("Page", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateContentItemFromContentTypeObject()
        {
            this.powerShell.Session.ProcessInput("Get-ContentType Page | New-ContentItem");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            Assert.Equal("0", table.Rows.Single()[0]);
            Assert.Equal("Page", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateDraftContentItem()
        {
            this.powerShell.Session.ProcessInput("New-ContentItem Page -Draft");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            int id = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(id > 0);
            Assert.Equal("Page", table.Rows.Single()[1]);

            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("(Get-ContentItem -Id " + id + " -VersionOptions Draft).VersionRecord.Published");
            Assert.Equal("False", this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }

        [Fact, Integration]
        public void ShouldCreatePublishedContentItem()
        {
            this.powerShell.Session.ProcessInput("New-ContentItem Page -Published");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            int id = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(id > 0);
            Assert.Equal("Page", table.Rows.Single()[1]);

            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("(Get-ContentItem -Id " + id + " -VersionOptions Published).VersionRecord.Published");
            Assert.Equal("True", this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }
    }
}