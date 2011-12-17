using Orchard.Management.PsProvider.Agents;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.PowerShell.Configuration {
    public class ConfigurationPsNavigationProvider : IPsNavigationProvider {
        public string Path {
            get {
                return "\\";
            }
        }

        public NodeType NodeType {
            get { return NodeType.Site; }
        }

        public OrchardVfsNode Node {
            get { return new ConfigurationNode(AgentManager); }
        }

        public void Initialize() { }

        public IAgentManager AgentManager { get; set; }
    }
}