using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.MetaData;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Items;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Content.Nodes {
    /// <summary>
    /// Implements a VFS node which contains content type definitions for an Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Edit-ContentType")]
    [SupportedCmdlet("Get-ContentPart")]
    [SupportedCmdlet("Add-ContentPart")]
    [SupportedCmdlet("Remove-ContentPart")]
    [SupportedCmdlet("New-ContentItem")]
    public class ContentTypesNode : ContainerNode {
        public ContentTypesNode(IPowerShellVfs vfs)
            : base(vfs, "Types") {
            Item = new CollectionItem(this) {
                Name = "Types",
                Description = "Contains the definitions of content types in the current tenant."
            };
        }

        public override IEnumerable<VfsNode> GetVirtualNodes() {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null) {
                return new VfsNode[0];
            }

            return this.UsingWorkContextScope(
                tenantName,
                scope => {
                    return scope.Resolve<IContentDefinitionManager>()
                        .ListTypeDefinitions()
                        .Select(definition => new ContentTypeNode(Vfs, definition))
                        .ToArray();
                });
        }
    }
}