namespace Proligence.PowerShell.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.Extensions.Models;

    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Utilities;

    /// <summary>
    /// Implements the <c>Get-OrchardFeature</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OrchardFeature", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardFeature : OrchardCmdlet, ITenantFilterCmdlet
    {
        /// <summary>
        /// The modules agent instance.
        /// </summary>
        private IModulesAgent modulesAgent;

        /// <summary>
        /// The tenants agent instance.
        /// </summary>
        private ITenantAgent tenantsAgent;

        /// <summary>
        /// All available Orchard tenants.
        /// </summary>
        private ShellSettings[] tenants;

        /// <summary>
        /// Gets or sets the name of the Orchard feature to get.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which the feature will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = false)]        
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which features will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether features should be retrieved from all tenants.
        /// </summary>
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public SwitchParameter FromAllTenants { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only enabled features should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only disabled features should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Disabled { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            
            this.tenants = tenantsAgent
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

            var features = new List<FeatureDescriptor>();

            foreach (ShellSettings tenant in filteredTenants)
            {
                var tenantFeatures = this.modulesAgent.GetFeatures(tenant.Name);
                features.AddRange(tenantFeatures);
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                features = features.Where(f => f.Name.WildcardEquals(this.Name)).ToList();
            }

            //if (this.Enabled.ToBool())
            //{
            //    features = features.Where(f => f.Enabled).ToList();
            //}

            //if (this.Disabled.ToBool())
            //{
            //    features = features.Where(f => !f.Enabled).ToList();
            //}

            foreach (FeatureDescriptor feature in features)
            {
                this.WriteObject(feature);
            }
        }
    }
}