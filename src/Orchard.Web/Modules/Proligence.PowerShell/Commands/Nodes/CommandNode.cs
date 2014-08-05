namespace Proligence.PowerShell.Commands.Nodes 
{
    using Proligence.PowerShell.Commands.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents a legacy Orchard command.
    /// </summary>
    [SupportedCmdlet("Invoke-OrchardCommand")]
    public class CommandNode : ObjectNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="command">The object of the legacy Orchard command represented by the node.</param>
        public CommandNode(IPowerShellVfs vfs, OrchardCommand command) 
            : base(vfs, command.CommandName, command)
        {
        }
    }
}