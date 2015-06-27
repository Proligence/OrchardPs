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
            this.powerShell.Execute("Remove-ContentItem " + id + " -Verbose");
            
            Assert.Equal(
                "Performing the operation \"Remove\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldDestroyContentItemById()
        {
            int id = this.CreateContentItem();
            this.powerShell.Execute("Remove-ContentItem " + id + " -Destroy -Verbose");
            
            Assert.Equal(
                "Performing the operation \"Destroy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldRemoveContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.powerShell.Execute("Get-ContentItem -Id " + id + " | Remove-ContentItem -Verbose");
            
            Assert.Equal(
                "Performing the operation \"Remove\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemDoesNotExist(id);
        }

        [Fact, Integration]
        public void ShouldDestroyContentItemByObject()
        {
            int id = this.CreateContentItem();
            this.powerShell.Execute("Get-ContentItem -Id " + id + " | Remove-ContentItem -Destroy -Verbose");
            
            Assert.Equal(
                "Performing the operation \"Destroy\" on target \"Content Item: " + id + ", Version: 1, Tenant: Default\".",
                this.powerShell.ConsoleConnection.VerboseOutput.ToString().Trim());

            this.AssertContentItemDoesNotExist(id);
        }

        private int CreateContentItem()
        {
            this.powerShell.Execute("$item = New-ContentItem Page -Published");
            var output = this.powerShell.Execute("$item.Id");
            this.powerShell.ConsoleConnection.Reset();
            
            return Convert.ToInt32(output);
        }

        private void AssertContentItemDoesNotExist(int id)
        {
            this.powerShell.ConsoleConnection.Reset();
            Assert.Empty(this.powerShell.Execute("Get-ContentItem " + id));
        }
    }
}