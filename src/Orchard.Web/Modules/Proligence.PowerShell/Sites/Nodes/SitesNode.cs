using System.Collections.Generic;
using System.Linq;
using Orchard.Management.PsProvider.Vfs;
using Orchard.PowerShell.AgentProxies;
using Orchard.PowerShell.Common;
using Orchard.PowerShell.Sites.Items;

namespace Orchard.PowerShell.Sites.Nodes {
    public class SitesNode : ContainerNode {
        private readonly TenantAgentProxy _tenantAgent;

        public SitesNode(TenantAgentProxy tenantAgent) : base("Sites") {
            _tenantAgent = tenantAgent;

            Item = new CollectionItem(this) {
                Name = "Sites",
                Description = "Contains all sites (tenants) of the Orchard instance."
            };
        }

        public override IEnumerable<OrchardVfsNode> GetVirtualNodes() {
            OrchardSite[] sites = _tenantAgent.GetSites();
            return sites.Select(site => new SiteNode(site));
        }
    }
}