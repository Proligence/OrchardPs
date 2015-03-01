namespace Proligence.PowerShell.Content.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.ContentManagement.MetaData;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Utilities;

    /// <summary>
    /// Implements a VFS node which contains content type definitions for an Orchard tenant.
    /// </summary>
    public class ContentTypesNode : ContainerNode
    {
        public ContentTypesNode(IPowerShellVfs vfs)
            : base(vfs, "Types")
        {
            this.Item = new CollectionItem(this)
            {
                Name = "Types",
                Description = "Contains the definitions of content types in the current tenant."
            };
        }

        public override IEnumerable<VfsNode> GetVirtualNodes()
        {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null)
            {
                return new VfsNode[0];
            }

            return this.UsingWorkContextScope(
                tenantName,
                scope =>
                    {
                        return scope.Resolve<IContentDefinitionManager>()
                            .ListTypeDefinitions()
                            .Select(definition => new ContentTypeNode(this.Vfs, definition))
                            .ToArray();
                    });
        }
    }
}