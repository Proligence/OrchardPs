using System.Management.Automation;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Recipes.Cmdlets {
    [OrchardFeature("Proligence.PowerShell.Recipes")]
    [Cmdlet(VerbsLifecycle.Invoke, "OrchardRecipe", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class InvokeOrchardRecipe : TenantCmdlet {
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "RecipeObject", Mandatory = true, ValueFromPipeline = true)]
        public Recipe RecipeObject { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "RecipeObject", Mandatory = false)]
        public override string Tenant { get; set; }

        protected override void ProcessRecord(ShellSettings tenant) {
            this.UsingWorkContextScope(tenant.Name, scope => {
                var recipe = GetRecipe(scope);
                if (recipe != null) {
                    string recipeName = RecipeObject != null
                        ? RecipeObject.Name
                        : Name;
                    if (ShouldProcess("Recipe: " + recipeName, "Invoke")) {
                        var recipeManager = scope.Resolve<IRecipeManager>();
                        recipeManager.Execute(recipe);
                    }
                }
            });
        }

        private Recipe GetRecipe(IWorkContextScope scope) {
            if (RecipeObject != null) {
                return RecipeObject;
            }

            var extensionManager = scope.Resolve<IExtensionManager>();
            var harvester = scope.Resolve<IRecipeHarvester>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (ExtensionDescriptor extension in extensionManager.AvailableExtensions()) {
                foreach (Recipe recipe in harvester.HarvestRecipes(extension.Id)) {
                    if (recipe.Name == Name) {
                        return recipe;
                    }
                }
            }

            WriteError(Error.InvalidArgument(
                "Failed to find recipe with name '" + Name + "'.",
                "FailedToFindExtension"));

            return null;
        }
    }
}