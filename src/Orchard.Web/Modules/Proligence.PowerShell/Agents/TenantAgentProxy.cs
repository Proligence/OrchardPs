using Orchard.Management.PsProvider.Agents;
using Proligence.PowerShell.Sites.Items;

namespace Proligence.PowerShell.Agents {
    [Agent("Proligence.PowerShell.Agents.TenantAgent, Proligence.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class TenantAgentProxy : AgentProxy {
        public OrchardSite[] GetSites() {
            return (OrchardSite[])Invoke("GetSites");
        }
    }
}