using Orchard.Recipes.Models;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Recipes.Nodes {
    public class RecipeNode : ObjectNode {
        public RecipeNode(IPowerShellVfs vfs, Recipe recipe)
            : base(vfs, recipe.Name, recipe) {
        }
    }
}