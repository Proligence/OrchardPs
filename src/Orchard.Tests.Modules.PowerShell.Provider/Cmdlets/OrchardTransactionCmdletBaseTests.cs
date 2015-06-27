namespace Orchard.Tests.Modules.PowerShell.Provider.Cmdlets
{
    using System;
    using Orchard.Tests.PowerShell.Infrastructure;
    using Xunit;

    [Collection("PowerShell")]
    public class OrchardTransactionCmdletBaseTests : IClassFixture<PowerShellFixture>
    {
        private readonly PowerShellFixture powerShell;

        public OrchardTransactionCmdletBaseTests(PowerShellFixture powerShell)
        {
            this.powerShell = powerShell;
            this.powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCompleteTransaction()
        {
            this.powerShell.Execute("Start-OrchardTransaction");
            
            string title = Guid.NewGuid().ToString("N");
            this.CreateContentItem(title);

            var table = this.powerShell.ExecuteTable("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Equal(1, table.Rows.Count);

            this.powerShell.Execute("Complete-OrchardTransaction");
            table = this.powerShell.ExecuteTable("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Equal(1, table.Rows.Count);
        }

        [Fact, Integration]
        public void ShouldUndoTransaction()
        {
            this.powerShell.Execute("Start-OrchardTransaction");
            
            string title = Guid.NewGuid().ToString("N");
            this.CreateContentItem(title);
            
            var table = this.powerShell.ExecuteTable("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Equal(1, table.Rows.Count);

            this.powerShell.Execute("Undo-OrchardTransaction");
            this.powerShell.ConsoleConnection.Reset();
            var output = this.powerShell.Execute("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Empty(output);
        }

        private void CreateContentItem(string title)
        {
            this.powerShell.Execute("$x = New-ContentItem -ContentType Page -Draft");
            this.powerShell.Execute("$x.TitlePart.Title = '" + title + "'");
            this.powerShell.Execute("$x | Publish-ContentItem");
        }
    }
}