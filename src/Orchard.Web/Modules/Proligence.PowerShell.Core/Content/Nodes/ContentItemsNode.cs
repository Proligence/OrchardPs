using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Items;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Content.Nodes {
    public class ContentItemsNode : ContainerNode {
        public ContentItemsNode(IPowerShellVfs vfs)
            : base(vfs, "Items") {
            Item = new CollectionItem(this) {
                Name = "Items",
                Description = "Contains the content items in the current tenant."
            };
        }

        public override IEnumerable<VfsNode> GetVirtualNodes() {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null) {
                return Enumerable.Empty<VfsNode>();
            }

            return this.UsingWorkContextScope(
                tenantName,
                scope => {
                    return scope.Resolve<IContentDefinitionManager>()
                        .ListTypeDefinitions()
                        .Select(definition => new ContentItemTypeNode(Vfs, this, definition))
                        .ToArray();
                });
        }
    }
}