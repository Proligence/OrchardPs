namespace Proligence.PowerShell.Recipes.Nodes
{
    using Orchard.Recipes.Models;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class RecipeNode : ObjectNode
    {
        public RecipeNode(IPowerShellVfs vfs, Recipe recipe)
            : base(vfs, recipe.Name, recipe)
        {
        }
    }
}