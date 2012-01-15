using Orchard.Management.PsProvider;
using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Sites.Items;
using Proligence.PowerShell.Sites.Nodes;

namespace Proligence.PowerShell.Common.Extensions {
    public static class OrchardCmdletExtensions {
        public static OrchardSite GetCurrentSite(this OrchardCmdlet cmdlet) {
            OrchardVfsNode currentNode = cmdlet.CurrentNode;
            while (currentNode != null) {
                var siteNode = currentNode as SiteNode;
                if (siteNode != null) {
                    return siteNode.Item as OrchardSite;
                }
                
                currentNode = currentNode.Parent;
            }

            return null;
        }
    }
}