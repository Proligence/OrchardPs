using System.Collections.Generic;
using System.Linq;
using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Agents;
using Proligence.PowerShell.Commands.Items;
using Proligence.PowerShell.Common.Extensions;
using Proligence.PowerShell.Common.Items;

namespace Proligence.PowerShell.Commands.Nodes {
    public class CommandsNode : ContainerNode {
        private readonly CommandAgentProxy _commandAgent;

        public CommandsNode(IOrchardVfs vfs, CommandAgentProxy commandAgent) : base(vfs, "Commands") {
            _commandAgent = commandAgent;

            Item = new CollectionItem(this) {
                Name = "Commands",
                Description = "Contains all legacy Orchard commands available in the current site."
            };
        }

        public override IEnumerable<OrchardVfsNode> GetVirtualNodes() {
            string siteName = this.GetCurrentSiteName();
            if (siteName == null) {
                return new OrchardVfsNode[0];
            }

            OrchardCommand[] commands = _commandAgent.GetCommands(siteName);
            return commands.Select(command => new CommandNode(Vfs, command));
        }
    }
}