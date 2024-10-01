using Proligence.PowerShell.Core.Content.Nodes;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Content.NavigationProviders {
    public class ContentPsNavigationProvider : PsNavigationProvider {
        public ContentPsNavigationProvider()
            : base(NodeType.Tenant) {
        }

        protected override void InitializeInternal() {
            Node = new ContentNode(Vfs);
        }
    }
}