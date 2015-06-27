namespace Proligence.PowerShell.Core.Recipes.NavigationProviders
{
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Recipes.Nodes;

    [OrchardFeature("Proligence.PowerShell.Recipes")]
    public class RecipePsNavigationProvider : PsNavigationProvider
    {
        public RecipePsNavigationProvider()
            : base(NodeType.Tenant)
        {
        }

        protected override void InitializeInternal()
        {
            this.Node = new RecipesNode(this.Vfs);
        }
    }
}