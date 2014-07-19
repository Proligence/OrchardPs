namespace Proligence.PowerShell.Content.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Content.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which contains content part definitions for an Orchard tenant.
    /// </summary>
    public class ContentPartsNode : ContainerNode
    {
        /// <summary>
        /// The content agent instance.
        /// </summary>
        private readonly IContentAgent contentAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPartsNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="contentAgent">The content agent instance.</param>
        public ContentPartsNode(IPowerShellVfs vfs, IContentAgent contentAgent)
            : base(vfs, "Parts")
        {
            this.contentAgent = contentAgent;

            this.Item = new CollectionItem(this)
            {
                Name = "Parts",
                Description = "Contains the definitions of content parts in the current tenant."
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

            OrchardContentPartDefinition[] parts = this.contentAgent.GetContentPartDefinitions(tenantName);
            return parts.Select(definition => new ContentPartNode(this.Vfs, definition));
        }
    }
}