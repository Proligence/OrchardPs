namespace Proligence.PowerShell.Core.Content.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.ContentManagement.MetaData;
    using Proligence.PowerShell.Core.Common.Items;
    using Proligence.PowerShell.Core.Utilities;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    
    /// <summary>
    /// Implements a VFS node which contains content field definitions for an Orchard tenant.
    /// </summary>
    public class ContentFieldsNode : ContainerNode
    {
        public ContentFieldsNode(IPowerShellVfs vfs)
            : base(vfs, "Fields") 
        {
            this.Item = new CollectionItem(this) 
            {
                Name = "Fields",
                Description = "Contains the definitions of content fields in the current tenant."
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
                            .ListFieldDefinitions()
                            .Select(definition => new ContentFieldNode(this.Vfs, definition))
                            .ToArray();
                    });
        }
    }
}