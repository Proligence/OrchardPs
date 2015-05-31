namespace Proligence.PowerShell.Core.Content.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.ContentManagement.MetaData;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Items;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    
    /// <summary>
    /// Implements a VFS node which contains content part definitions for an Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Edit-ContentPartDefinition")]
    [SupportedCmdlet("Add-ContentField")]
    [SupportedCmdlet("Remove-ContentField")]
    public class ContentPartsNode : ContainerNode
    {
        public ContentPartsNode(IPowerShellVfs vfs)
            : base(vfs, "Parts")
        {
            this.Item = new CollectionItem(this)
            {
                Name = "Parts",
                Description = "Contains the definitions of content parts in the current tenant."
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
                            .ListPartDefinitions()
                            .Select(definition => new ContentPartNode(this.Vfs, definition))
                            .ToArray();
                    });
        }
    }
}