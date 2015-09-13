using System.Linq;
using System.Management.Automation;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("gcpd")]
    [Cmdlet(VerbsCommon.Get, "ContentPartDefinition", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetContentPartDefinition : TenantCmdlet {
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        protected override void ProcessRecord(ShellSettings tenant) {
            ContentPartDefinition[] contentPartDefinitions = this.UsingWorkContextScope(
                tenant.Name,
                scope => scope.Resolve<IContentDefinitionManager>().ListPartDefinitions().ToArray());

            foreach (var definition in contentPartDefinitions) {
                if (!string.IsNullOrEmpty(Name)) {
                    if (!definition.Name.WildcardEquals(Name)) {
                        continue;
                    }
                }

                WriteObject(definition);
            }
        }
    }
}