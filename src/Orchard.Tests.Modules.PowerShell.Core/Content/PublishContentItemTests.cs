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
            this.powerShell.Session.ProcessInput("Publish-ContentItem " + id + " -Verbose");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            Assert.Equal(
                "Performing the operation \"Publish\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemPublished(id);
        }

        [Fact, Integration]
        public void ShouldPublishContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput(
                "Get-ContentItem -Id " + id + " -VersionOptions Latest | Publish-ContentItem -Verbose");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            Assert.Equal(
                "Performing the operation \"Publish\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemPublished(id);
        }

        private int CreateContentItem()
        {
            this.powerShell.Session.ProcessInput("$item = New-ContentItem Page -Draft");
            this.powerShell.Session.ProcessInput("$item.Id");
            string output = this.powerShell.ConsoleConnection.Output.ToString().Trim();
            this.powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }

        private void AssertContentItemPublished(int id)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("(Get-ContentItem -Id " + id + " -VersionOptions Latest).VersionRecord.Published");
            Assert.Equal("True", this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }
    }
}