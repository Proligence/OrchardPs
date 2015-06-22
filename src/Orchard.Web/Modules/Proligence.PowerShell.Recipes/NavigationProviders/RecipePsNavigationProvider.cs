namespace Proligence.PowerShell.Recipes.NavigationProviders
{
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Recipes.Nodes;

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