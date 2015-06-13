namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    public abstract class RetrieveOrchardFeatureCmdletBase<TFeature> : TenantCmdlet
    {
        private IList<PSObject> features;

        [Parameter(ParameterSetName = "Id", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Id { get; set; }

        [Alias("n")]
        [Parameter(ParameterSetName = "Name", Mandatory = true)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "Id", Mandatory = false)]
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public override string Tenant { get; set; }

        [Alias("e")]
        [Parameter(ParameterSetName = "Id", Mandatory = false)]
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        [Alias("d")]
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
            this.features = new List<PSObject>();
        }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            foreach (TFeature feature in this.GetFeatures(tenant.Name))
            {
                bool enabled = this.IsFeatureEnabled(feature, tenant.Name);

                if (this.Enabled.ToBool() && !enabled)
                {
                    continue;
                }

                if (this.Disabled.ToBool() && enabled)
                {
                    continue;
                }

                var psobj = PSObject.AsPSObject(feature);
                psobj.Properties.Add(new PSNoteProperty("Enabled", enabled));

                this.features.Add(psobj);
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            
            if (!string.IsNullOrEmpty(this.Id))
            {
                this.features = this.features
                    .Where(f => this.GetFeatureId((TFeature)f.ImmediateBaseObject).WildcardEquals(this.Id))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                this.features = this.features
                    .Where(f => this.GetFeatureName((TFeature)f.ImmediateBaseObject).WildcardEquals(this.Name))
                    .ToList();
            }

            foreach (PSObject feature in this.features)
            {
                this.WriteObject(feature);
            }
        }
    }
}