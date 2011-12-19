using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Agents;
using Proligence.PowerShell.Commands.Nodes;

namespace Proligence.PowerShell.Commands.NavigationProviders {
    public class CommandsPsNavigationProvider : PsNavigationProvider {
        public override void Initialize()
        {
            NodeType = NodeType.Site;
            Node = new CommandsNode(Vfs, AgentManager.GetAgent<CommandAgentProxy>());
        }
    }
}