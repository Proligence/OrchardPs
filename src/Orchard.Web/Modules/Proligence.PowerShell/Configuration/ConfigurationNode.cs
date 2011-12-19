using Orchard.Management.PsProvider.Vfs;

namespace Proligence.PowerShell.Configuration {
    public class ConfigurationNode : ContainerNode {
        public ConfigurationNode(IOrchardVfs vfs) : base(vfs, "Configuration") {
            Item = "Configuration";
        }
    }
}