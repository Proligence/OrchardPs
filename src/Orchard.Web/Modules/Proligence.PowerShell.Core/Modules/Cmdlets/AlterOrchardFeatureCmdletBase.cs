using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Modules.Cmdlets {
    public abstract class AlterOrchardFeatureCmdletBase<TFeature> : TenantCmdlet {
        /// <summary>
        /// Cached list of all Orchard features for each tenant.
        /// </summary>
        private IDictionary<string, TFeature[]> _cache;

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public string Id { get; set; }

        [Alias("f")]
        [ValidateNotNull]
        [Parameter(ParameterSetName = "FeatureObject", ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public TFeature Feature { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "FeatureObject", ValueFromPipeline = true)]
        public override string Tenant { get; set; }

        protected override void BeginProcessing() {
            base.BeginProcessing();
            _cache = new Dictionary<string, TFeature[]>();
        }

        protected abstract IEnumerable<TFeature> GetTenantFeatures(string tenantName);
        protected abstract string GetFeatureId(TFeature feature);
        protected abstract string GetActionName(TFeature feature, string tenantName);
        protected abstract void PerformAction(TFeature feature, string tenantName);

        protected override void ProcessRecord(ShellSettings tenant) {
            TFeature feature = GetFeature(tenant);
            if (feature != null) {
                if (ShouldProcess(
                    "Feature: " + GetFeatureId(feature) + ", Tenant: " + tenant.Name,
                    GetActionName(feature, tenant.Name))) {
                    PerformAction(feature, tenant.Name);
                }
            }
        }

        private TFeature GetFeature(ShellSettings tenant) {
            if (ParameterSetName == "FeatureObject") {
                return Feature;
            }

            TFeature[] tenantFeatures;
            if (!_cache.TryGetValue(tenant.Name, out tenantFeatures)) {
                tenantFeatures = GetTenantFeatures(tenant.Name).ToArray();
                _cache.Add(tenant.Name, tenantFeatures);
            }

            var feature = tenantFeatures.FirstOrDefault(
                f => GetFeatureId(f).Equals(Id, StringComparison.CurrentCultureIgnoreCase));

            if (feature == null) {
                WriteError(Error.InvalidArgument(
                    "Failed to find feature with name '" + Id + "'.",
                    "FailedToFindFeature"));
            }

            return feature;
        }
    }
}