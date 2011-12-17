using Orchard.Management.PsProvider.Vfs;
using Orchard.PowerShell.Sites.Items;

namespace Orchard.PowerShell.Sites.Nodes {
    public class SiteNode : ContainerNode {
        public SiteNode(OrchardSite site) : base(site.Name) {
            Item = site;
        }
    }
}