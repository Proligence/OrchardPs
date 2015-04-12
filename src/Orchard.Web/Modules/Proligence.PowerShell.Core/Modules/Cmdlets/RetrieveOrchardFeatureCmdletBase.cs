namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    public abstract class RetrieveOrchardFeatureCmdletBase<TFeature> : OrchardCmdlet, ITenantFilterCmdlet
    {
        protected ShellSettings[] Tenants { get; private set; }

        [Parameter(ParameterSetName = "Id", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Id { get; set; }

        [Parameter(ParameterSetName = "Name", Mandatory = true)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "Id", Mandatory = false)]
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public SwitchParameter FromAllTenants { get; set; }

        [Parameter(ParameterSetName = "Id", Mandatory = false)]
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        [Parameter(ParameterSetName = "Id", Mandatory = false)]
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Disabled { get; set; }

        protected abstract IEnumerable<TFeature> GetFeatures(string tenantName);
        protected abstract string GetFeatureId(TFeature feature);
        protected abstract string GetFeatureName(TFeature feature);
        protected abstract bool IsFeatureEnabled(TFeature feature, string tenantName);

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            this.Tenants = this.Resolve<IShellSettingsManager>()
                .LoadSettings()
                .Where(t => t.State == TenantState.Running)
                .ToArray();
        }

        protected override void ProcessRecord()
        {
            IEnumerable<ShellSettings> filteredTenants = CmdletHelper.FilterTenants(this, this.Tenants);

            var features = new List<TFeature>();

            foreach (ShellSettings tenant in filteredTenants)
            {
                foreach (TFeature feature in this.GetFeatures(tenant.Name))
                {
                    if (this.Enabled.ToBool())
                    {
                        if (!this.IsFeatureEnabled(feature, tenant.Name))
                        {
                            continue;
                        }
                    }

                    if (this.Disabled.ToBool())
                    {
                        if (this.IsFeatureEnabled(feature, tenant.Name))
                        {
                            continue;
                        }
                    }

                    features.Add(feature);
                }
            }

            if (!string.IsNullOrEmpty(this.Id))
            {
                features = features.Where(f => this.GetFeatureId(f).WildcardEquals(this.Id)).ToList();
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                features = features.Where(f => this.GetFeatureName(f).WildcardEquals(this.Name)).ToList();
            }

            foreach (TFeature feature in features)
            {
                this.WriteObject(feature);
            }
        }
    }
}