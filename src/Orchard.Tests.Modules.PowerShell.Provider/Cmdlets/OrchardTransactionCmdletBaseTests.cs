using System;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Provider.Cmdlets {
    [Collection("PowerShell")]
    public class OrchardTransactionCmdletBaseTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public OrchardTransactionCmdletBaseTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void ShouldCompleteTransaction() {
            _powerShell.Execute("Start-OrchardTransaction");

            string title = Guid.NewGuid().ToString("N");
            CreateContentItem(title);

            var table =
                _powerShell.ExecuteTable("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Equal(1, table.Rows.Count);

            _powerShell.Execute("Complete-OrchardTransaction");
            table = _powerShell.ExecuteTable("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Equal(1, table.Rows.Count);
        }

        [Fact, Integration]
        public void ShouldUndoTransaction() {
            _powerShell.Execute("Start-OrchardTransaction");

            string title = Guid.NewGuid().ToString("N");
            CreateContentItem(title);

            var table =
                _powerShell.ExecuteTable("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Equal(1, table.Rows.Count);

            _powerShell.Execute("Undo-OrchardTransaction");
            _powerShell.ConsoleConnection.Reset();
            var output = _powerShell.Execute("Get-ContentItem -ContentType Page | where { $_.Title -eq '" + title + "' }");
            Assert.Empty(output);
        }

        private void CreateContentItem(string title) {
            _powerShell.Execute("$x = New-ContentItem -ContentType Page -Draft");
            _powerShell.Execute("$x.TitlePart.Title = '" + title + "'");
            _powerShell.Execute("$x | Publish-ContentItem");
        }
    }
}