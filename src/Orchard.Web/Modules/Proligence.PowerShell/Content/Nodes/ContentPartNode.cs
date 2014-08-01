namespace Proligence.PowerShell.Content.Nodes
{
    using System.Diagnostics.CodeAnalysis;

    using Orchard.ContentManagement.MetaData.Models;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard content part definition.
    /// </summary>
    [SupportedCmdlet("Edit-ContentPartDefinition")]
    public class ContentPartNode : ObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPartNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="definition">The object which represents the content part definition.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public ContentPartNode(IPowerShellVfs vfs, ContentPartDefinition definition)
            : base(vfs, definition.Name, definition)
        {
        }
    }
}