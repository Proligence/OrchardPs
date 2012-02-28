// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Commands.Nodes 
{
    using Orchard.Management.PsProvider.Vfs;
    using Proligence.PowerShell.Commands.Items;

    /// <summary>
    /// Implements a VFS node which represents a legacy Orchard command.
    /// </summary>
    public class CommandNode : ObjectNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="command">The object of the legacy Orchard command represented by the node.</param>
        public CommandNode(IOrchardVfs vfs, OrchardCommand command) 
            : base(vfs, command.CommandName, command)
        {
        }
    }
}