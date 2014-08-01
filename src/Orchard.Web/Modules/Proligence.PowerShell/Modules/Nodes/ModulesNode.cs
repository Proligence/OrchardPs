namespace Proligence.PowerShell.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="ModuleNode"/> nodes for a single Orchard tenant.
    /// </summary>
    public class ModulesNode : ContainerNode
    {
        /// <summary>
        /// The command agent instance.
        /// </summary>
        private readonly IModulesAgent modulesAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModulesNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="modulesAgent">The modules agent instance.</param>
        public ModulesNode(IPowerShellVfs vfs, IModulesAgent modulesAgent)
            : base(vfs, "Modules") 
        {
            this.modulesAgent = modulesAgent;

            this.Item = new CollectionItem(this) 
            {
                Name = "Modules",
                Description = "Contains all modules available in the current tenant."
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

            var modules = this.modulesAgent.GetModules(tenantName);
            return modules.Select(module => new ModuleNode(this.Vfs, module));
        }
    }
}