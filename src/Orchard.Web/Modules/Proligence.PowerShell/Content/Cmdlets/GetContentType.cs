namespace Proligence.PowerShell.Content.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;
    using Proligence.PowerShell.Utilities;

    [CmdletAlias("gct")]
    [Cmdlet(VerbsCommon.Get, "ContentType", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetContentType : OrchardCmdlet, ITenantFilterCmdlet
    {
        private ShellSettings[] tenants;

        /// <summary>
        /// Gets or sets the name of the content type which definition will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which content type definitions will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which content type definitions will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether content type definitions should be retrieved from all tenants.
        /// </summary>
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public SwitchParameter FromAllTenants { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            this.tenants = this.Resolve<IShellSettingsManager>()
                .LoadSettings()
                .Where(t => t.State == TenantState.Running)
                .ToArray();
        }

        protected override void ProcessRecord()
        {
            IEnumerable<ShellSettings> filteredTenants = CmdletHelper.FilterTenants(this, this.tenants);

            var definitions = new List<ContentTypeDefinition>();

            foreach (var tenant in filteredTenants)
            {
                definitions.AddRange(
                    this.UsingWorkContextScope(
                        tenant.Name,
                        scope => scope.Resolve<IContentDefinitionManager>().ListTypeDefinitions().ToArray()));
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                definitions = definitions.Where(f => f.Name.WildcardEquals(this.Name)).ToList();
            }

            foreach (ContentTypeDefinition definition in definitions)
            {
                this.WriteObject(definition);
            }
        }
    }
}