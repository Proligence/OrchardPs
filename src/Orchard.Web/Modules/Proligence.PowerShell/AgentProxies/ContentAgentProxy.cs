using Orchard.Management.PsProvider.Agents;

namespace Orchard.PowerShell.AgentProxies {
    [Agent("Orchard.PowerShell.Agents.ContentAgent, Orchard.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class ContentAgentProxy : AgentProxy {
        public string[] Hello() {
            return (string[])Invoke("Hello");
        }
    }
}