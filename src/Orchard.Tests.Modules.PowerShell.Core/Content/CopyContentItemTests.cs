namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using System.Linq;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class CopyContentItemTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public CopyContentItemTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCopyContentItemById()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput("Copy-ContentItem " + id + " -Verbose");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            Assert.Equal(
                "Performing the operation \"Copy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            var newId = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(newId > id);
        }

        [Fact, Integration]
        public void ShouldCopyContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput(
                "Get-ContentItem -Id " + id + " -VersionOptions Latest | Copy-ContentItem -Verbose");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            Assert.Equal(
                "Performing the operation \"Copy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString());
            var newId = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(newId > id);
        }

        private int CreateContentItem()
        {
            this.powerShell.Session.ProcessInput("$item = New-ContentItem Page -Published");
            this.powerShell.Session.ProcessInput("$item.Id");
            string output = this.powerShell.ConsoleConnection.Output.ToString().Trim();
            this.powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }
    }
}