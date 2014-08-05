namespace Proligence.PowerShell.Commands.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Commands;
    using Proligence.PowerShell.Commands.Items;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Utilities;

    /// <summary>
    /// Implements a VFS node which groups <see cref="CommandNode"/> nodes for a single Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Invoke-OrchardCommand")]
    public class CommandsNode : ContainerNode 
    {
        public CommandsNode(IPowerShellVfs vfs)
            : base(vfs, "Commands")
        {
            this.Item = new CollectionItem(this) 
            {
                Name = "Commands",
                Description = "Contains all legacy Orchard commands available in the current tenant."
            };
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>A sequence of child nodes.</returns>
        public override IEnumerable<VfsNode> GetVirtualNodes() 
        {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null) 
            {
                return new VfsNode[0];
            }

            return this.GetCommands(tenantName).Select(
                command => new CommandNode(this.Vfs, command));
        }

        /// <summary>
        /// Gets all legacy commands which are available for the specified Orchard tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>
        /// An array of <see cref="OrchardCommand"/> objects which represent the Orchard commands which are available
        /// at the specified tenant.
        /// </returns>
        private IEnumerable<OrchardCommand> GetCommands(string tenant)
        {
            return this.UsingWorkContextScope(
                tenant,
                scope =>
                    {
                        var commandManager = scope.Resolve<ICommandManager>();
                        IEnumerable<CommandDescriptor> commandDescriptors = commandManager.GetCommandDescriptors();
                        IEnumerable<OrchardCommand> commands = commandDescriptors.Select(
                            command => new OrchardCommand
                            {
                                CommandName = command.Name,
                                HelpText = command.HelpText,
                                TenantName = tenant
                            });

                        return commands.ToArray();
                    });
        }
    }
}