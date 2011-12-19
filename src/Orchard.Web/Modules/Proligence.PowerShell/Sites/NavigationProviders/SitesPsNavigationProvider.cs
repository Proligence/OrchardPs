using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Agents;
using Proligence.PowerShell.Sites.Nodes;

namespace Proligence.PowerShell.Sites.NavigationProviders {
    public class SitesPsNavigationProvider : PsNavigationProvider {
        public override void Initialize()
        {
            NodeType = NodeType.Global;
            Node = new SitesNode(Vfs, AgentManager.GetAgent<TenantAgentProxy>());
        }
    }
}