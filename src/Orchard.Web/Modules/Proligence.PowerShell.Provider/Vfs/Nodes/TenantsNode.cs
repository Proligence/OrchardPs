using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider.Vfs.Items;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Provider.Vfs.Nodes {
    /// <summary>
    /// Implements a VFS node which groups <see cref="TenantNode"/> nodes for a single Orchard installation.
    /// </summary>
    [SupportedCmdlet("Enable-Tenant")]
    [SupportedCmdlet("Disable-Tenant")]
    [SupportedCmdlet("Remove-Tenant")]
    [SupportedCmdlet("Edit-Tenant")]
    public class TenantsNode : ContainerNode {
        private readonly IShellSettingsManager _shellSettingsManager;

        public TenantsNode(IPowerShellVfs vfs, IShellSettingsManager shellSettingsManager)
            : base(vfs, "Tenants") {
            _shellSettingsManager = shellSettingsManager;

            Item = new CollectionItem(this) {
                Name = "Tenants",
                Description = "Contains all tenants of the Orchard instance."
            };
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() {
            ShellSettings[] tenants = _shellSettingsManager.LoadSettings().ToArray();
            return tenants.Select(tenant => new TenantNode(Vfs, tenant));
        }
    }
}