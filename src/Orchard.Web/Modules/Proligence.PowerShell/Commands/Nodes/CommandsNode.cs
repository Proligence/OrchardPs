// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Commands.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Commands.Items;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="CommandNode"/> nodes for a single Orchard site.
    /// </summary>
    [SupportedCmdlet("Invoke-OrchardCommand")]
    public class CommandsNode : ContainerNode 
    {
        /// <summary>
        /// The command agent instance.
        /// </summary>
        private readonly ICommandAgent commandAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="commandAgent">The command agent instance.</param>
        public CommandsNode(IPowerShellVfs vfs, ICommandAgent commandAgent)
            : base(vfs, "Commands") 
        {
            this.commandAgent = commandAgent;

            this.Item = new CollectionItem(this) 
            {
                Name = "Commands",
                Description = "Contains all legacy Orchard commands available in the current site."
            };
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>A sequence of child nodes.</returns>
        public override IEnumerable<VfsNode> GetVirtualNodes() 
        {
            string siteName = this.GetCurrentSiteName();
            if (siteName == null) 
            {
                return new VfsNode[0];
            }

            OrchardCommand[] commands = this.commandAgent.GetCommands(siteName);
            return commands.Select(command => new CommandNode(Vfs, command));
        }
    }
}