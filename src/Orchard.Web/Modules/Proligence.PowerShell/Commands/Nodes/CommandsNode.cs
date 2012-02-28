﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Commands.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Management.PsProvider.Vfs;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Commands.Items;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Common.Items;

    /// <summary>
    /// Implements a VFS node which groups <see cref="CommandNode"/> nodes for a single Orchard site.
    /// </summary>
    public class CommandsNode : ContainerNode 
    {
        /// <summary>
        /// The command agent proxy instance.
        /// </summary>
        private readonly CommandAgentProxy commandAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="commandAgent">The command agent proxy instance.</param>
        public CommandsNode(IOrchardVfs vfs, CommandAgentProxy commandAgent)
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
        public override IEnumerable<OrchardVfsNode> GetVirtualNodes() 
        {
            string siteName = this.GetCurrentSiteName();
            if (siteName == null) 
            {
                return new OrchardVfsNode[0];
            }

            OrchardCommand[] commands = this.commandAgent.GetCommands(siteName);
            return commands.Select(command => new CommandNode(Vfs, command));
        }
    }
}