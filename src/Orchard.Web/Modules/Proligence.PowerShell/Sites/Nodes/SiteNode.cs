using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Sites.Items;

namespace Proligence.PowerShell.Sites.Nodes {
    public class SiteNode : ContainerNode {
        public SiteNode(OrchardSite site) : base(site.Name) {
            Item = site;
        }
    }
}