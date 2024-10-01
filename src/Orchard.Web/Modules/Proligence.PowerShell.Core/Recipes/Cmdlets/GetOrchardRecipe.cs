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

namespace Proligence.PowerShell.Core.Recipes.Cmdlets {
    [OrchardFeature("Proligence.PowerShell.Recipes")]
    [Cmdlet(VerbsCommon.Get, "OrchardRecipe", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardRecipe : TenantCmdlet {
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

        protected override void ProcessRecord(ShellSettings tenant) {
            this.UsingWorkContextScope(tenant.Name, scope => {
                var extensionManager = scope.Resolve<IExtensionManager>();
                var harvester = scope.Resolve<IRecipeHarvester>();

                foreach (ExtensionDescriptor extension in GetExtensions(extensionManager)) {
                    foreach (Recipe recipe in harvester.HarvestRecipes(extension.Id)) {
                        if (string.IsNullOrEmpty(Name) || recipe.Name.WildcardEquals(Name)) {
                            WriteObject(recipe);
                        }
                    }
                }
            });
        }

        private IEnumerable<ExtensionDescriptor> GetExtensions(IExtensionManager extensionManager) {
            if (ExtensionId != null) {
                ExtensionDescriptor extension = extensionManager.GetExtension(ExtensionId);
                if (extension == null) {
                    WriteError(Error.InvalidArgument(
                        "Failed to find extension with ID '" + ExtensionId + "'.",
                        "FailedToFindExtension"));

                    return Enumerable.Empty<ExtensionDescriptor>();
                }

                return new[] {extension};
            }

            return extensionManager.AvailableExtensions().ToArray();
        }
    }
}