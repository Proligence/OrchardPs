using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Commands.Items;

namespace Proligence.PowerShell.Commands.Nodes {
    public class CommandNode : ObjectNode {
        public CommandNode(IOrchardVfs vfs, OrchardCommand command) : base(vfs, command.CommandName, command) { }
    }
}