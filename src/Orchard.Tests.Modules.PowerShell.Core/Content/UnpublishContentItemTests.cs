namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class UnpublishContentItemTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public UnpublishContentItemTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldUnpublishContentItemById()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput("Unpublish-ContentItem " + id + " -Verbose");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            Assert.Equal(
                "Performing the operation \"Unpublish\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemNotPublished(id);
        }

        [Fact, Integration]
        public void ShouldUnpublishContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput(
                "Get-ContentItem -Id " + id + " -VersionOptions Latest | Unpublish-ContentItem -Verbose");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            Assert.Equal(
                "Performing the operation \"Unpublish\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemNotPublished(id);
        }

        private int CreateContentItem()
        {
            this.powerShell.Session.ProcessInput("$item = New-ContentItem Page -Published");
            this.powerShell.Session.ProcessInput("$item.Id");
            string output = this.powerShell.ConsoleConnection.Output.ToString().Trim();
            this.powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }

        private void AssertContentItemNotPublished(int id)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("(Get-ContentItem -Id " + id + " -VersionOptions Latest).VersionRecord.Published");
            Assert.Equal("False", this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }
    }
}