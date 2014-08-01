namespace Proligence.PowerShell.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="ThemeNode"/> nodes for a single Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardTheme")]
    public class ThemesNode : ContainerNode
    {
        /// <summary>
        /// The command agent instance.
        /// </summary>
        private readonly IModulesAgent modulesAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemesNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="modulesAgent">The modules agent instance.</param>
        public ThemesNode(IPowerShellVfs vfs, IModulesAgent modulesAgent)
            : base(vfs, "Themes") 
        {
            this.modulesAgent = modulesAgent;

            this.Item = new CollectionItem(this) 
            {
                Name = "Themes",
                Description = "Contains all themes available in the current tenant."
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

            OrchardTheme[] themes = this.modulesAgent.GetThemes(tenantName);
            return themes.Select(theme => new ThemeNode(this.Vfs, theme));
        }
    }
}