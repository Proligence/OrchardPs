// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentPartNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Content.Nodes
{
    using System.Diagnostics.CodeAnalysis;
    using Proligence.PowerShell.Content.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard content part definition.
    /// </summary>
    public class ContentPartNode : ObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPartNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="definition">The object which represents the content part definition.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public ContentPartNode(IPowerShellVfs vfs, OrchardContentPartDefinition definition)
            : base(vfs, definition.Name, definition)
        {
        }
    }
}