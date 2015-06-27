namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class UpdateContentItemTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public UpdateContentItemTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldUpdateContentItem()
        {
            this.powerShell.Execute("$item = New-ContentItem Page -Published");
            var title = Guid.NewGuid().ToString("N");
            this.powerShell.Execute("$item.TitlePart.Title = '" + title + "'");
            int id = Convert.ToInt32(this.powerShell.Execute("$item.Id"));
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Execute("Update-ContentItem $item");
            Assert.Equal(title, this.powerShell.Execute("(Get-ContentItem -Id " + id + ").Title"));
        }

        [Fact, Integration]
        public void ShouldUpdateLatestVersionOfContentItem()
        {
            this.powerShell.Execute("$item = New-ContentItem Page -Published");
            var title = Guid.NewGuid().ToString("N");
            this.powerShell.Execute("$item.TitlePart.Title = '" + title + "'");
            int id = Convert.ToInt32(this.powerShell.Execute("$item.Id"));
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Execute("Update-ContentItem $item -VersionOptions Latest -Verbose");
            string verbose = this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim();
            Assert.Equal("Performing the operation \"Update Latest\" on target \"Content Item: " + id + ", Tenant: Default\".", verbose);
            
            this.powerShell.ConsoleConnection.Reset();
            Assert.Equal(title, this.powerShell.Execute("(Get-ContentItem -Id " + id + ").Title"));
        }
    }
}