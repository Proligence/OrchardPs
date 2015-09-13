using Proligence.PowerShell.Core.Commands.Nodes;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Commands.NavigationProviders {
    public class CommandsPsNavigationProvider : PsNavigationProvider {
        public CommandsPsNavigationProvider()
            : base(NodeType.Tenant) {
        }

        protected override void InitializeInternal() {
            Node = new CommandsNode(Vfs);
        }
    }
}