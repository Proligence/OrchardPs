namespace Proligence.PowerShell.Recipes.Cmdlets
{
    using System.Management.Automation;
    using Orchard;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Recipes.Models;
    using Orchard.Recipes.Services;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsLifecycle.Invoke, "OrchardRecipe", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class InvokeOrchardRecipe : TenantCmdlet
    {
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

        protected override void ProcessRecord(ShellSettings tenant)
        {
            this.UsingWorkContextScope(tenant.Name, scope =>
            {
                var recipe = this.GetRecipe(scope);
                if (recipe != null)
                {
                    string recipeName = this.RecipeObject != null ? this.RecipeObject.Name : this.Name;
                    if (this.ShouldProcess("Recipe: " + recipeName, "Invoke"))
                    {
                        var recipeManager = scope.Resolve<IRecipeManager>();
                        recipeManager.Execute(recipe);
                    }
                }
            });
        }

        private Recipe GetRecipe(IWorkContextScope scope)
        {
            if (this.RecipeObject != null)
            {
                return this.RecipeObject;
            }

            var extensionManager = scope.Resolve<IExtensionManager>();
            var harvester = scope.Resolve<IRecipeHarvester>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (ExtensionDescriptor extension in extensionManager.AvailableExtensions())
            {
                foreach (Recipe recipe in harvester.HarvestRecipes(extension.Id))
                {
                    if (recipe.Name == this.Name)
                    {
                        return recipe;
                    }
                }
            }

            this.WriteError(Error.InvalidArgument(
                "Failed to find recipe with name '" + this.Name + "'.",
                "FailedToFindExtension"));

            return null;
        }
    }
}