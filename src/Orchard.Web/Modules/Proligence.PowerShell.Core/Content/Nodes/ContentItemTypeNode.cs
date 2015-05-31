namespace Proligence.PowerShell.Core.Content.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.MetaData.Models;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Items;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class ContentItemTypeNode : ContainerNode
    {
        private readonly ContentTypeDefinition definition;

        public ContentItemTypeNode(IPowerShellVfs vfs, VfsNode parentNode, ContentTypeDefinition definition)
            : base(vfs, definition.Name)
        {
            this.Parent = parentNode;
            this.definition = definition;
            
            this.Item = new CollectionItem(this)
            {
                Name = definition.Name,
                Description = "Contains content items of type: " + definition.DisplayName
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
                    return scope.Resolve<IContentManager>()
                        .Query(this.definition.Name)
                        .ForVersion(VersionOptions.Latest)
                        .List()
                        .Select(item => new ContentItemNode(this.Vfs, this, item))
                        .ToArray();
                });
        }
    }
}