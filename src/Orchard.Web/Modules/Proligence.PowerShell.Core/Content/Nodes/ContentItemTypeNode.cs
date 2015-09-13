using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Items;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Content.Nodes {
    [SupportedCmdlet("Update-ContentItem")]
    [SupportedCmdlet("Remove-ContentItem")]
    [SupportedCmdlet("Publish-ContentItem")]
    [SupportedCmdlet("Unpublish-ContentItem")]
    [SupportedCmdlet("Copy-ContentItem")]
    [SupportedCmdlet("Restore-ContentItem")]
    public class ContentItemTypeNode : ContainerNode {
        private readonly ContentTypeDefinition _definition;

        public ContentItemTypeNode(IPowerShellVfs vfs, VfsNode parentNode, ContentTypeDefinition definition)
            : base(vfs, definition.Name) {
            Parent = parentNode;
            _definition = definition;

            Item = new CollectionItem(this) {
                Name = definition.Name,
                Description = "Contains content items of type: " + definition.DisplayName
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
                    return scope.Resolve<IContentManager>()
                        .Query(_definition.Name)
                        .ForVersion(VersionOptions.Latest)
                        .List()
                        .Select(item => new ContentItemNode(Vfs, this, item))
                        .ToArray();
                });
        }
    }
}