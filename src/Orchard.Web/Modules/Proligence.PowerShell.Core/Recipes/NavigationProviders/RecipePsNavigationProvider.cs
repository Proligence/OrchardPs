using Orchard.Environment.Extensions;
using Proligence.PowerShell.Core.Recipes.Nodes;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Recipes.NavigationProviders {
    [OrchardFeature("Proligence.PowerShell.Recipes")]
    public class RecipePsNavigationProvider : PsNavigationProvider {
        public RecipePsNavigationProvider()
            : base(NodeType.Tenant) {
        }

        protected override void InitializeInternal() {
            Node = new RecipesNode(Vfs);
        }
    }
}