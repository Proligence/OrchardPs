namespace Orchard.Tests.Modules.PowerShell.Core.Content
{
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class RemoveContentTypeTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public RemoveContentTypeTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldRemoveContentTypeByName()
        {
            this.EnsureContentTypeExists("Foo");
            this.powerShell.Execute("Remove-ContentType Foo");
            Assert.Empty(this.powerShell.Execute("Get-ContentType Foo"));
        }

        [Fact, Integration]
        public void ShouldRemoveContentTypeByObject()
        {
            this.EnsureContentTypeExists("Foo");
            this.powerShell.Execute("Get-ContentType Foo | Remove-ContentType");
            Assert.Empty(this.powerShell.Execute("Get-ContentType Foo"));
        }

        private void EnsureContentTypeExists(string name)
        {
            this.powerShell.ConsoleConnection.Reset();
            var output = this.powerShell.Execute("Get-ContentType " + name);
            if (string.IsNullOrEmpty(output))
            {
                this.powerShell.Execute("New-ContentType " + name);
                this.powerShell.ConsoleConnection.Reset();
                Assert.NotEmpty(this.powerShell.Execute("Get-ContentType " + name));
            }

            this.powerShell.ConsoleConnection.Reset();
        }
    }
}