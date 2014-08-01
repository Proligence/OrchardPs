namespace Proligence.PowerShell.Content.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Utilities;

    /// <summary>
    /// Implements the <c>Get-ContentPartDefinition</c> cmdlet.
    /// </summary>
    [CmdletAlias("gcpd")]
    [Cmdlet(VerbsCommon.Get, "ContentPartDefinition", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetContentPartDefinition : OrchardCmdlet, ITenantFilterCmdlet
    {
        private IContentAgent contentAgent;
        private ITenantAgent tenantAgent;
        private ShellSettings[] tenants;

        /// <summary>
        /// Gets or sets the name of the content part which definition will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which content part definitions will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which content part definitions will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating content part definitions should be retrieved from all tenants.
        /// </summary>
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public SwitchParameter FromAllTenants { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            this.tenants = this.tenantAgent
                .GetTenants()
                .Where(t => t.State == TenantState.Running)
                .ToArray();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            IEnumerable<ShellSettings> filteredTenants = CmdletHelper.FilterTenants(this, this.tenants);

            var definitions = new List<ContentPartDefinition>();

            foreach (var tenant in filteredTenants)
            {
                definitions.AddRange(this.contentAgent.GetContentPartDefinitions(tenant.Name));
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                definitions = definitions.Where(f => f.Name.WildcardEquals(this.Name)).ToList();
            }

            foreach (ContentPartDefinition definition in definitions)
            {
                this.WriteObject(definition);
            }
        }
    }
}