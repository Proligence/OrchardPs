namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [CmdletAlias("gct")]
    [Cmdlet(VerbsCommon.Get, "ContentType", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetContentType : TenantCmdlet
    {
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            ContentTypeDefinition[] definitions = this.UsingWorkContextScope(
                tenant.Name,
                scope => scope.Resolve<IContentDefinitionManager>().ListTypeDefinitions().ToArray());

            foreach (var definition in definitions)
            {
                if (!string.IsNullOrEmpty(this.Name))
                {
                    if (!definition.Name.WildcardEquals(this.Name))
                    {
                        continue;
                    }
                }

                this.WriteObject(definition);
            }
        }
    }
}