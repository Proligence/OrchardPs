using System.Collections.Generic;
using Orchard.Management.PsProvider.Agents;
using Proligence.PowerShell.Commands.Items;

namespace Proligence.PowerShell.Agents {
    [Agent("Proligence.PowerShell.Agents.CommandAgent, Proligence.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class CommandAgentProxy : AgentProxy {

        public OrchardCommand[] GetCommands(string site) {
            return (OrchardCommand[])Invoke("GetCommands", site);
        }

        public void ExecuteCommand(string siteName, string[] args, Dictionary<string, string> switches) {
            Invoke("ExecuteCommand", siteName, args, switches);
        }
    }
}