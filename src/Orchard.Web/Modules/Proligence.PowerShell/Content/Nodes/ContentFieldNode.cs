namespace Proligence.PowerShell.Content.Nodes
{
    using System.Diagnostics.CodeAnalysis;

    using Orchard.ContentManagement.MetaData.Models;

    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard content field definition.
    /// </summary>
    public class ContentFieldNode : ObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFieldNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="definition">The object which represents the content field definition.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public ContentFieldNode(IPowerShellVfs vfs, ContentFieldDefinition definition)
            : base(vfs, definition.Name, definition)
        {
        }
    }
}