namespace Proligence.PowerShell.Core.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Items;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    
    /// <summary>
    /// Implements a VFS node which groups <see cref="FeatureNode"/> nodes for a single Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardFeature")]
    [SupportedCmdlet("Disable-OrchardFeature")]
    public class FeaturesNode : ContainerNode
    {
        private readonly IExtensionManager extensions;

        public FeaturesNode(IPowerShellVfs vfs, IExtensionManager extensions)
            : base(vfs, "Features")
        {
            this.extensions = extensions;
            this.Item = new CollectionItem(this) 
            {
                Name = "Features",
                Description = "Contains all features available in the current tenant."
            };
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() 
        {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null) 
            {
                return new VfsNode[0];
            }

            return this.extensions.AvailableFeatures().Select(
                feature => new FeatureNode(this.Vfs, feature));
        }
    }
}