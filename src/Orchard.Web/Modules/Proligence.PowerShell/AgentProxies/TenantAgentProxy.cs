using Orchard.Management.PsProvider.Agents;
using Orchard.PowerShell.Sites.Items;

namespace Orchard.PowerShell.AgentProxies {
    [Agent("Orchard.PowerShell.Agents.TenantAgent, Orchard.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class TenantAgentProxy : AgentProxy {
        public OrchardSite[] GetSites() {
            return (OrchardSite[])Invoke("GetSites");
        }
    }
}