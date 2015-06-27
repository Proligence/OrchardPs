namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using System;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class RemoveContentItemTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public RemoveContentItemTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldRemoveContentItemById()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput("Remove-ContentItem " + id + " -Verbose");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            Assert.Equal(
                "Performing the operation \"Remove\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldDestroyContentItemById()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput("Remove-ContentItem " + id + " -Destroy -Verbose");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            Assert.Equal(
                "Performing the operation \"Destroy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldRemoveContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput("Get-ContentItem -Id " + id + " | Remove-ContentItem -Verbose");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            Assert.Equal(
                "Performing the operation \"Remove\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldDestroyContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.powerShell.Session.ProcessInput("Get-ContentItem -Id " + id + " | Remove-ContentItem -Destroy -Verbose");
            this.powerShell.ConsoleConnection.AssertNoErrors();

            Assert.Equal(
                "Performing the operation \"Destroy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemDoesNotExist(id);
        }

        private int CreateContentItem()
        {
            this.powerShell.Session.ProcessInput("$item = New-ContentItem Page -Published");
            this.powerShell.Session.ProcessInput("$item.Id");
            string output = this.powerShell.ConsoleConnection.Output.ToString().Trim();
            this.powerShell.ConsoleConnection.Reset();
            
            return Convert.ToInt32(output);
        }

        private void AssertContentItemDoesNotExist(int id)
        {
            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-ContentItem " + id);
            Assert.Empty(this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }
    }
}