namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Environment.Features;
    using Proligence.PowerShell.Provider;

    [Cmdlet(VerbsLifecycle.Enable, "OrchardFeature", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EnableOrchardFeature : OrchardCmdlet
    {
        /// <summary>
        /// Cached list of all Orchard features for each tenant.
        /// </summary>
        private IDictionary<string, FeatureDescriptor[]> features;

        /// <summary>
        /// Gets or sets the name of the feature to enable.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which the feature will be enabled.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FeatureDescriptor"/> object which represents the orchard feature to enable.
        /// </summary>
        [Parameter(ParameterSetName = "FeatureObject", ValueFromPipeline = true)]
        public FeatureDescriptor Feature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the feature should not enable it's dependencies and fail if they
        /// are required.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "FeatureObject", Mandatory = false)]
        public SwitchParameter WithoutDependencies { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.features = new Dictionary<string, FeatureDescriptor[]>();
        }

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

            FeatureDescriptor feature = null;

            if (this.ParameterSetName == "Default")
            {
                feature = this.GetOrchardFeature(tenantName, this.Name);
                if (feature == null)
                {
                    this.WriteError(
                        new ArgumentException("Failed to find feature with name '" + this.Name + "'."),
                        "FailedToFindFeature",
                        ErrorCategory.InvalidArgument);
                }
            }
            else if (this.ParameterSetName == "FeatureObject")
            {
                feature = this.Feature;
            }

            if (feature != null)
            {
                if (this.ShouldProcess("Feature: " + feature.Id + ", Tenant: " + tenantName, "Enable Feature"))
                {
                    this.UsingWorkContextScope(
                        tenantName,
                        scope =>
                            {
                                var featureManager = this.Resolve<IFeatureManager>();
                                
                                featureManager.EnableFeatures(
                                    new[] { feature.Id },
                                    !this.WithoutDependencies.ToBool());
                            });
                }
            }
        }

        private FeatureDescriptor GetOrchardFeature(string tenantName, string featureName)
        {
            FeatureDescriptor[] tenantFeatures;

            if (!this.features.TryGetValue(tenantName, out tenantFeatures))
            {
                tenantFeatures = this.UsingWorkContextScope(
                    tenantName,
                    scope =>
                        {
                            var extensionManager = scope.Resolve<IExtensionManager>();
                            return extensionManager.AvailableFeatures().ToArray();
                        });

                this.features.Add(tenantName, tenantFeatures);
            }

            return tenantFeatures.FirstOrDefault(
                f => f.Name.Equals(featureName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}