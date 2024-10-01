﻿using System.Linq;
using Orchard.Tests.PowerShell.Infrastructure;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Core.Commands {
    [Collection("PowerShell")]
    public class CommandVfsTests : IClassFixture<PowerShellFixture> {
        private readonly PowerShellFixture _powerShell;

        public CommandVfsTests(PowerShellFixture powerShell) {
            _powerShell = powerShell;
            _powerShell.ConsoleConnection.Reset();
        }

        [Fact, Integration]
        public void VfsTenantShouldContainCommands() {
            var table = _powerShell.ExecuteTable("Get-ChildItem Orchard:\\Tenants\\Default\\Commands");

            Assert.Equal("Name", table.Header[0]);
            Assert.Equal("Help", table.Header[1]);
            Assert.True(table.Rows.Count > 0);
            Assert.Equal(1, table.Rows.Count(x => x[0] == "help"));
            Assert.Equal(1, table.Rows.Count(x => x[0] == "help commands"));
        }
    }
}