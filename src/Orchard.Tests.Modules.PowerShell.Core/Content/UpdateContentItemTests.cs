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
            this.powerShell.Session.ProcessInput("$item = New-ContentItem Page -Published");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            
            var title = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("$item.TitlePart.Title = '" + title + "'");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            this.powerShell.Session.ProcessInput("$item.Id");
            int id = Convert.ToInt32(this.powerShell.ConsoleConnection.Output.ToString());
            this.powerShell.ConsoleConnection.Reset();

            this.powerShell.Session.ProcessInput("Update-ContentItem $item");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            this.powerShell.Session.ProcessInput("(Get-ContentItem -Id " + id + ").Title");
            string output = this.powerShell.ConsoleConnection.Output.ToString().Trim();

            Assert.Equal(title, output);
        }

        [Fact, Integration]
        public void ShouldUpdateLatestVersionOfContentItem()
        {
            this.powerShell.Session.ProcessInput("$item = New-ContentItem Page -Published");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            var title = Guid.NewGuid().ToString("N");
            this.powerShell.Session.ProcessInput("$item.TitlePart.Title = '" + title + "'");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            this.powerShell.Session.ProcessInput("$item.Id");
            int id = Convert.ToInt32(this.powerShell.ConsoleConnection.Output.ToString());
            this.powerShell.ConsoleConnection.Reset();

            this.powerShell.Session.ProcessInput("Update-ContentItem $item -VersionOptions Latest -Verbose");
            this.powerShell.ConsoleConnection.AssertNoErrors();
            string verbose = this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim();
            Assert.Equal("Performing the operation \"Update Latest\" on target \"Content Item: " + id + ", Tenant: Default\".", verbose);
            this.powerShell.ConsoleConnection.Reset();

            this.powerShell.Session.ProcessInput("(Get-ContentItem -Id " + id + ").Title");
            string output = this.powerShell.ConsoleConnection.Output.ToString().Trim();
            Assert.Equal(title, output);
        }
    }
}