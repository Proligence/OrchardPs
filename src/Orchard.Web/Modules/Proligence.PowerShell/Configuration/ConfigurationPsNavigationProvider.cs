using Orchard.Management.PsProvider.Vfs;

namespace Proligence.PowerShell.Configuration {
    public class ConfigurationPsNavigationProvider : PsNavigationProvider {
        public override void Initialize() {
            NodeType = NodeType.Site;
            Node = new ConfigurationNode(Vfs);
        }
    }
}