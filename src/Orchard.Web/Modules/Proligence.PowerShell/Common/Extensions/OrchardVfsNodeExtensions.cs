using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Sites.Items;
using Proligence.PowerShell.Sites.Nodes;

namespace Proligence.PowerShell.Common.Extensions {
    public static class OrchardVfsNodeExtensions {
        public static string GetCurrentSiteName(this OrchardVfsNode node) {
            while (node != null) {
                var siteNode = node as SiteNode;
                if (siteNode != null) {
                    return ((OrchardSite)siteNode.Item).Name;
                }

                node = node.Parent;
            }

            return null;
        }
    }
}
