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

            this.powerShell.Session.ProcessInput("Remove-ContentType Foo");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("Get-ContentType Foo");
            Assert.Empty(this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }

        [Fact, Integration]
        public void ShouldRemoveContentTypeByObject()
        {
            this.EnsureContentTypeExists("Foo");

            this.powerShell.Session.ProcessInput("Get-ContentType Foo | Remove-ContentType");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("Get-ContentType Foo");
            Assert.Empty(this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }

        private void EnsureContentTypeExists(string name)
        {
            this.powerShell.Session.ProcessInput("Get-ContentType " + name);
            string output = this.powerShell.ConsoleConnection.Output.ToString();

            if (string.IsNullOrEmpty(output))
            {
                this.powerShell.Session.ProcessInput("New-ContentType " + name);
                this.powerShell.ConsoleConnection.Reset();
                this.powerShell.Session.ProcessInput("Get-ContentType " + name);
                Assert.NotEmpty(this.powerShell.ConsoleConnection.Output.ToString());
            }

            this.powerShell.ConsoleConnection.Reset();
        }
    }
}