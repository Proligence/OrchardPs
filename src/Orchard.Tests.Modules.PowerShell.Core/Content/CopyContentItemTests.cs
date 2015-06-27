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
            var table = this.powerShell.ExecuteTable("Copy-ContentItem " + id + " -Verbose");
            
            Assert.Equal(
                "Performing the operation \"Copy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());
            
            var newId = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(newId > id);
        }

        [Fact, Integration]
        public void ShouldCopyContentItemByObject()
        {
            int id = this.CreateContentItem();
            
            var table = this.powerShell.ExecuteTable(
                "Get-ContentItem -Id " + id + " -VersionOptions Latest | Copy-ContentItem -Verbose");
            
            Assert.Equal(
                "Performing the operation \"Copy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            var newId = Convert.ToInt32(table.Rows.Single()[0]);
            Assert.True(newId > id);
        }

        private int CreateContentItem()
        {
            this.powerShell.Execute("$item = New-ContentItem Page -Published");
            var output = this.powerShell.Execute("$item.Id");
            this.powerShell.ConsoleConnection.Reset();

            return Convert.ToInt32(output);
        }
    }
}