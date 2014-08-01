namespace Proligence.PowerShell.Content.Nodes
{
    using System.Collections.Generic;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which contains content-related items of an Orchard tenant.
    /// </summary>
    public class ContentNode : ContainerNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="contentAgent">The content agent.</param>
        public ContentNode(IPowerShellVfs vfs, IContentAgent contentAgent)
            : base(vfs, "Content", CreateStaticNodes(vfs, contentAgent))
        {
            this.Item = new DescriptionItem
            {
                Name = "Content",
                Description = "Contains content definitions and data for the current tenant."
            };
        }

        /// <summary>
        /// Creates the static subnodes of the <see cref="ContentNode"/>.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="contentAgent">The content agent.</param>
        /// <returns>Sequence of created VFS nodes.</returns>
        private static IEnumerable<VfsNode> CreateStaticNodes(IPowerShellVfs vfs, IContentAgent contentAgent)
        {
            yield return new ContentFieldsNode(vfs, contentAgent);
            yield return new ContentPartsNode(vfs, contentAgent);
        }
    }
}