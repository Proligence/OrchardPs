using System.Collections.Generic;
using System.Linq;
using Orchard.Management.PsProvider.Agents;
using Orchard.Management.PsProvider.Vfs;
using Proligence.PowerShell.Agents;

namespace Proligence.PowerShell.Configuration {
    public class ConfigurationNode : ContainerNode {
        private readonly IAgentManager _agentManager;

        public ConfigurationNode(IAgentManager agentManager) : base("Configuration") {
            _agentManager = agentManager;
            
            Item = "Test object";
        }

        public override IEnumerable<OrchardVfsNode> GetVirtualNodes() {
            var contentAgent = _agentManager.GetAgent<ContentAgentProxy>();
            string[] arr = contentAgent.Hello();
            return arr.Select(str => new ObjectNode(str, str));
        }
    }
}