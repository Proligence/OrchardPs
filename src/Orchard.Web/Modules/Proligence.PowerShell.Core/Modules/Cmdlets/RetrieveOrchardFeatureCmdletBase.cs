using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Modules.Cmdlets {
    public abstract class RetrieveOrchardFeatureCmdletBase<TFeature> : TenantCmdlet {
        private IList<PSObject> _features;

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

        protected override void BeginProcessing() {
            base.BeginProcessing();
            _features = new List<PSObject>();
        }

        protected override void ProcessRecord(ShellSettings tenant) {
            foreach (TFeature feature in GetFeatures(tenant.Name)) {
                bool enabled = IsFeatureEnabled(feature, tenant.Name);

                if (Enabled.ToBool() && !enabled) {
                    continue;
                }

                if (Disabled.ToBool() && enabled) {
                    continue;
                }

                var psobj = PSObject.AsPSObject(feature);
                psobj.Properties.Add(new PSNoteProperty("Enabled", enabled));

                _features.Add(psobj);
            }
        }

        protected override void ProcessRecord() {
            base.ProcessRecord();

            if (!string.IsNullOrEmpty(Id)) {
                _features = _features
                    .Where(f => GetFeatureId((TFeature) f.ImmediateBaseObject).WildcardEquals(Id))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(Name)) {
                _features = _features
                    .Where(f => GetFeatureName((TFeature) f.ImmediateBaseObject).WildcardEquals(Name))
                    .ToList();
            }

            foreach (PSObject feature in _features) {
                WriteObject(feature);
            }
        }
    }
}