namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    public abstract class AlterOrchardFeatureCmdletBase<TFeature> : TenantCmdlet
    {
        /// <summary>
        /// Cached list of all Orchard features for each tenant.
        /// </summary>
        private IDictionary<string, TFeature[]> cache;

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public string Id { get; set; }

        [ValidateNotNull]
        [Parameter(ParameterSetName = "FeatureObject", ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public TFeature Feature { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "FeatureObject", ValueFromPipeline = true)]
        public override string Tenant { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.cache = new Dictionary<string, TFeature[]>();
        }

        protected abstract IEnumerable<TFeature> GetTenantFeatures(string tenantName);
        protected abstract string GetFeatureId(TFeature feature);
        protected abstract string GetActionName(TFeature feature, string tenantName);
        protected abstract void PerformAction(TFeature feature, string tenantName);

        protected override void ProcessRecord(ShellSettings tenant)
        {
            TFeature feature = this.GetFeature(tenant);
            if (feature != null)
            {
                if (this.ShouldProcess(
                    "Feature: " + this.GetFeatureId(feature) + ", Tenant: " + tenant.Name,
                    this.GetActionName(feature, tenant.Name)))
                {
                    this.PerformAction(feature, tenant.Name);
                }
            }
        }

        private TFeature GetFeature(ShellSettings tenant)
        {
            if (this.ParameterSetName == "FeatureObject")
            {
                return this.Feature;
            }

            TFeature[] tenantFeatures;
            if (!this.cache.TryGetValue(tenant.Name, out tenantFeatures))
            {
                tenantFeatures = this.GetTenantFeatures(tenant.Name).ToArray();
                this.cache.Add(tenant.Name, tenantFeatures);
            }

            var feature = tenantFeatures.FirstOrDefault(
                f => this.GetFeatureId(f).Equals(this.Id, StringComparison.CurrentCultureIgnoreCase));

            if (feature == null)
            {
                this.WriteError(Error.InvalidArgument(
                    "Failed to find feature with name '" + this.Id + "'.",
                    "FailedToFindFeature"));
            }
            
            return feature;
        }
    }
}