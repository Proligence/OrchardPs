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
            this.powerShell.Session.ProcessInput("Start-OrchardTransaction");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            string title = Guid.NewGuid().ToString("N");
            this.CreateContentItem(title);

            this.powerShell.Session.ProcessInput("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString().Trim());
            Assert.Equal(1, table.Rows.Count);

            this.powerShell.Session.ProcessInput("Complete-OrchardTransaction");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString().Trim());
            Assert.Equal(1, table.Rows.Count);
        }

        [Fact, Integration]
        public void ShouldUndoTransaction()
        {
            this.powerShell.Session.ProcessInput("Start-OrchardTransaction");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            string title = Guid.NewGuid().ToString("N");
            this.CreateContentItem(title);
            
            this.powerShell.Session.ProcessInput("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            var table = PsTable.Parse(this.powerShell.ConsoleConnection.Output.ToString().Trim());
            Assert.Equal(1, table.Rows.Count);

            this.powerShell.Session.ProcessInput("Undo-OrchardTransaction");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.ConsoleConnection.Reset();
            this.powerShell.Session.ProcessInput("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Empty(this.powerShell.ConsoleConnection.Output.ToString().Trim());
        }

        private void CreateContentItem(string title)
        {
            this.powerShell.Session.ProcessInput("$x = New-ContentItem -ContentType Page -Draft");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("$x.TitlePart.Title = '" + title + "'");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());

            this.powerShell.Session.ProcessInput("$x | Publish-ContentItem");
            Assert.Empty(this.powerShell.ConsoleConnection.ErrorOutput.ToString());
        }
    }
}