namespace Proligence.PowerShell.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="FeatureNode"/> nodes for a single Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardFeature")]
    [SupportedCmdlet("Disable-OrchardFeature")]
    public class FeaturesNode : ContainerNode
    {
        /// <summary>
        /// The command agent instance.
        /// </summary>
        private readonly IModulesAgent modulesAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeaturesNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="modulesAgent">The modules agent instance.</param>
        public FeaturesNode(IPowerShellVfs vfs, IModulesAgent modulesAgent)
            : base(vfs, "Features") 
        {
            this.modulesAgent = modulesAgent;

            this.Item = new CollectionItem(this) 
            {
                Name = "Features",
                Description = "Contains all features available in the current tenant."
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

            OrchardFeature[] features = this.modulesAgent.GetFeatures(tenantName);
            return features.Select(feature => new FeatureNode(this.Vfs, feature));
        }
    }
}