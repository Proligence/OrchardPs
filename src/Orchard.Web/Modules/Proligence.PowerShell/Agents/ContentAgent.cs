using Orchard.Management.PsProvider.Agents;

namespace Proligence.PowerShell.Agents {
    public class ContentAgent : AgentBase {
        public string[] Hello() {
            return new[] { "Hello", "World", "! "};
        }
    }
}