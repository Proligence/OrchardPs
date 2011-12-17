using Orchard.Management.PsProvider.Agents;

namespace Proligence.PowerShell.Agents {
    [Agent("Proligence.PowerShell.Agents.ContentAgent, Proligence.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class ContentAgentProxy : AgentProxy {
        public string[] Hello() {
            return (string[])Invoke("Hello");
        }
    }
}