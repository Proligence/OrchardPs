namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    public abstract class AlterOrchardFeatureCmdletBase<TFeature> : OrchardCmdlet
    {
        /// <summary>
        /// Cached list of all Orchard features for each tenant.
        /// </summary>
        private IDictionary<string, TFeature[]> cache;

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Id { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        [Parameter(ParameterSetName = "FeatureObject", ValueFromPipeline = true)]
        public TFeature Feature { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.cache = new Dictionary<string, TFeature[]>();
        }

        protected abstract IEnumerable<TFeature> GetTenantFeatures(string tenantName);
        protected abstract string GetFeatureId(TFeature feature);
        protected abstract string GetActionName(TFeature feature, string tenantName);
        protected abstract void PerformAction(TFeature feature, string tenantName);

        protected override void ProcessRecord()
        {
            string tenantName = null;

            if ((this.ParameterSetName != "FeatureObject") && string.IsNullOrEmpty(this.Tenant))
            {
                // Get feature for current tenant if tenant name not specified
                ShellSettings tenant = this.GetCurrentTenant();
                tenantName = tenant != null ? tenant.Name : "Default";
            }

            if (!string.IsNullOrEmpty(this.Tenant))
            {
                tenantName = this.Tenant;
            }

            TFeature feature = default(TFeature);

            if (this.ParameterSetName == "Default")
            {
                feature = this.GetFeature(tenantName, this.Id);
                if (feature == null)
                {
                    this.WriteError(Error.InvalidArgument(
                        "Failed to find feature with name '" + this.Id + "'.",
                        "FailedToFindFeature"));
                }
            }
            else if (this.ParameterSetName == "FeatureObject")
            {
                feature = this.Feature;
            }

            if (feature != null)
            {
                if (this.ShouldProcess(
                    "Feature: " + this.GetFeatureId(feature) + ", Tenant: " + tenantName,
                    this.GetActionName(feature, tenantName)))
                {
                    this.PerformAction(feature, tenantName);
                }
            }
        }

        private TFeature GetFeature(string tenantName, string featureId)
        {
            TFeature[] tenantFeatures;

            if (!this.cache.TryGetValue(tenantName, out tenantFeatures))
            {
                tenantFeatures = this.GetTenantFeatures(tenantName).ToArray();
                this.cache.Add(tenantName, tenantFeatures);
            }

            return tenantFeatures.FirstOrDefault(
                f => this.GetFeatureId(f).Equals(featureId, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}