namespace Proligence.PowerShell.Core.Content.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.ContentManagement.MetaData;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Items;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class ContentItemsNode : ContainerNode
    {
        public ContentItemsNode(IPowerShellVfs vfs)
            : base(vfs, "Items")
        {
            this.Item = new CollectionItem(this)
            {
                Name = "Items",
                Description = "Contains the content items in the current tenant."
            };
        }

        public override IEnumerable<VfsNode> GetVirtualNodes()
        {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null)
            {
                return Enumerable.Empty<VfsNode>();
            }

            return this.UsingWorkContextScope(
                tenantName,
                scope =>
                {
                    return scope.Resolve<IContentDefinitionManager>()
                        .ListTypeDefinitions()
                        .Select(definition => new ContentItemTypeNode(this.Vfs, this, definition))
                        .ToArray();
                });
        }
    }
}