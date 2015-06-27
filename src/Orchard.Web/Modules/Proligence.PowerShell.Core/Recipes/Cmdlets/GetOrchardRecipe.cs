namespace Proligence.PowerShell.Core.Recipes.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Recipes.Models;
    using Orchard.Recipes.Services;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [OrchardFeature("Proligence.PowerShell.Recipes")]
    [Cmdlet(VerbsCommon.Get, "OrchardRecipe", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardRecipe : TenantCmdlet
    {
        /// <remarks>
        /// This parameter doesn't make any sense for this cmdlet.
        /// </remarks>>
        public override SwitchParameter AllTenants { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        public string Name { get; set; }

        [Alias("e")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string ExtensionId { get; set; }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            this.UsingWorkContextScope(tenant.Name, scope =>
            {
                var extensionManager = scope.Resolve<IExtensionManager>();
                var harvester = scope.Resolve<IRecipeHarvester>();

                foreach (ExtensionDescriptor extension in this.GetExtensions(extensionManager))
                {
                    foreach (Recipe recipe in harvester.HarvestRecipes(extension.Id))
                    {
                        if (string.IsNullOrEmpty(this.Name) || recipe.Name.WildcardEquals(this.Name))
                        {
                            this.WriteObject(recipe);
                        }
                    }
                }
            });
        }

        private IEnumerable<ExtensionDescriptor> GetExtensions(IExtensionManager extensionManager)
        {
            if (this.ExtensionId != null)
            {
                ExtensionDescriptor extension = extensionManager.GetExtension(this.ExtensionId);
                if (extension == null)
                {
                    this.WriteError(Error.InvalidArgument(
                        "Failed to find extension with ID '" + this.ExtensionId + "'.",
                        "FailedToFindExtension"));

                    return Enumerable.Empty<ExtensionDescriptor>();
                }

                return new[] { extension };
            }
            
            return extensionManager.AvailableExtensions().ToArray();
        }
    }
}