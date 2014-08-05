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
        public CommandNode(IPowerShellVfs vfs, OrchardCommand command) 
            : base(vfs, command.CommandName, command)
        {
        }
    }
}