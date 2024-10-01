using Proligence.PowerShell.Core.Commands.Items;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Commands.Nodes {
    /// <summary>
    /// Implements a VFS node which represents a legacy Orchard command.
    /// </summary>
    [SupportedCmdlet("Invoke-OrchardCommand")]
    public class CommandNode : ObjectNode {
        public CommandNode(IPowerShellVfs vfs, OrchardCommand command)
            : base(vfs, command.CommandName, command) {
        }
    }
}