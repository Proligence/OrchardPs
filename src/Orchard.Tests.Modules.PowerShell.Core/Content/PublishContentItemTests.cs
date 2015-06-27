namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class PublishContentItemTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public PublishContentItemTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldPublishContentItemById()
        {
            int id = this.CreateContentItem();
            this.powerShell.Execute("Publish-ContentItem " + id + " -Verbose");
            
            Assert.Equal(
                "Performing the operation \"Publish\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemPublished(id);
        }

        [Fact, Integration]
        public void ShouldPublishContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.powerShell.Execute("Get-ContentItem -Id " + id + " -VersionOptions Latest | Publish-ContentItem -Verbose");

            Assert.Equal(
                "Performing the operation \"Publish\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemPublished(id);
        }

        private int CreateContentItem()
        {
            this.powerShell.Execute("$item = New-ContentItem Page -Draft");
            var output = this.powerShell.Execute("$item.Id");
            this.powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }

        private void AssertContentItemPublished(int id)
        {
            this.powerShell.ConsoleConnection.Reset();
            var output = this.powerShell.Execute("(Get-ContentItem -Id " + id + " -VersionOptions Latest).VersionRecord.Published");
            Assert.Equal("True", output);
        }
    }
}