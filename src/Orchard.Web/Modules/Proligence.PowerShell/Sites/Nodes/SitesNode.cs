using System.Collections.Generic;
using System.Linq;
using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Agents;
using Proligence.PowerShell.Common;
using Proligence.PowerShell.Sites.Items;

namespace Proligence.PowerShell.Sites.Nodes {
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