namespace Proligence.PowerShell.Recipes.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Environment.Extensions;
    using Orchard.Recipes.Services;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Items;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class RecipesNode : ContainerNode
    {
        public RecipesNode(IPowerShellVfs vfs)
            : base(vfs, "Recipes")
        {
            this.Item = new CollectionItem(this)
            {
                Name = "Recipes",
                Description = "Contains the recipes available in the current tenant."
            };
        }

        public override IEnumerable<VfsNode> GetVirtualNodes()
        {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null)
            {
                return new VfsNode[0];
            }

            return this.UsingWorkContextScope(
                tenantName,
                scope =>
                {
                    var extensionManager = scope.Resolve<IExtensionManager>();
                    var harvester = scope.Resolve<IRecipeHarvester>();
                    
                    return (from extension in extensionManager.AvailableExtensions()
                            from recipe in harvester.HarvestRecipes(extension.Id)
                            select new RecipeNode(this.Vfs, recipe)).ToList();
                });
        }
    }
}