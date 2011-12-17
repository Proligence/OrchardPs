using Orchard.Management.PsProvider.Vfs;
using Orchard.PowerShell.AgentProxies;
using Orchard.PowerShell.Sites.Nodes;

namespace Orchard.PowerShell.Sites.NavigationProviders {
    public class SitesPsNavigationProvider : PsNavigationProvider {
        public override void Initialize()
        {
            NodeType = NodeType.Global;
            Path = "\\";
            Node = new SitesNode(AgentManager.GetAgent<TenantAgentProxy>());
        }
    }
}