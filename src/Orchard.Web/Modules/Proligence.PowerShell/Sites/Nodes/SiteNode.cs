using System;
using System.Collections.Generic;
using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Sites.Items;

namespace Proligence.PowerShell.Sites.Nodes {
    public class SiteNode : ContainerNode {
        private List<OrchardVfsNode> _siteNodes;

        public SiteNode(IOrchardVfs vfs, OrchardSite site) : base(vfs, site.Name) {
            Item = site;
        }

        public override IEnumerable<OrchardVfsNode> GetVirtualNodes() {
            if (_siteNodes == null) {
                _siteNodes = new List<OrchardVfsNode>();

                IEnumerable<IPsNavigationProvider> siteNavigationProviders = Vfs.NavigationProviderManager.GetProviders(NodeType.Site);
                foreach (IPsNavigationProvider navigationProvider in siteNavigationProviders) {
                    if (navigationProvider.Path != "\\") {
                        throw new NotSupportedException("Only root paths are supported for site navigation providers.");
                    }

                    _siteNodes.Add(navigationProvider.Node);
                }
            }

            return _siteNodes.ToArray();
        }
    }
}