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
            var table = this.powerShell.ExecuteTable("New-ContentItem Page");
            Assert.Equal("0", table.Rows.Single()[0]);
            Assert.Equal("Page", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateContentItemFromContentTypeObject()
        {
            var table = this.powerShell.ExecuteTable("Get-ContentType Page | New-ContentItem");
            Assert.Equal("0", table.Rows.Single()[0]);
            Assert.Equal("Page", table.Rows.Single()[1]);
        }

        [Fact, Integration]
        public void ShouldCreateDraftContentItem()
        {
            var table = this.powerShell.ExecuteTable("New-ContentItem Page -Draft");
            int id = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(id > 0);
            Assert.Equal("Page", table.Rows.Single()[1]);

            this.powerShell.ConsoleConnection.Reset();
            var output = this.powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Draft).VersionRecord.Published");
            Assert.Equal("False", output);
        }

        [Fact, Integration]
        public void ShouldCreatePublishedContentItem()
        {
            var table = this.powerShell.ExecuteTable("New-ContentItem Page -Published");
            int id = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(id > 0);
            Assert.Equal("Page", table.Rows.Single()[1]);

            this.powerShell.ConsoleConnection.Reset();
            var output = this.powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Published).VersionRecord.Published");
            Assert.Equal("True", output);
        }
    }
}