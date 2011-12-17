using Orchard.Management.PsProvider.Agents;

namespace Orchard.PowerShell.Agents {
    public class ContentAgent : AgentBase {
        public string[] Hello() {
            return new[] { "Hello", "World", "! "};
        }
    }
}