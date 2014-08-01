namespace Proligence.PowerShell.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    using Orchard.Environment.Configuration;
    using Orchard.Environment.Extensions.Models;

    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements the <c>Disable-OrchardFeature</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Disable, "OrchardFeature", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class DisableOrchardFeature : OrchardCmdlet
    {
        /// <summary>
        /// The modules proxy instance.
        /// </summary>
        private IModulesAgent modulesAgent;

        /// <summary>
        /// Cached list of all Orchard features for each tenant.
        /// </summary>
        private IDictionary<string, FeatureDescriptor[]> features;

        /// <summary>
        /// Gets or sets the name of the feature to disable.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which the feature will be disabled.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OrchardFeature"/> object which represents the orchard feature to disable.
        /// </summary>
        [Parameter(ParameterSetName = "FeatureObject", ValueFromPipeline = true)]
        public FeatureDescriptor Feature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the feature should not disable it's dependencies and fail if they
        /// are required.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "FeatureObject", Mandatory = false)]
        public SwitchParameter WithoutDependencies { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.features = new Dictionary<string, FeatureDescriptor[]>();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
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
                if (this.ShouldProcess("Feature: " + feature.Id + ", Tenant: " + tenantName, "Disable Feature"))
                {
                    this.modulesAgent.DisableFeature(tenantName, feature.Id, !this.WithoutDependencies.ToBool());
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="OrchardFeature"/> object for the specified feature.
        /// </summary>
        /// <param name="tenantName">The name to the tenant for which to get feature.</param>
        /// <param name="featureName">The name of the feature to get.</param>
        /// <returns>The <see cref="OrchardFeature"/> object for the specified feature.</returns>
        private FeatureDescriptor GetOrchardFeature(string tenantName, string featureName)
        {
            FeatureDescriptor[] tenantFeatures;

            if (!this.features.TryGetValue(tenantName, out tenantFeatures))
            {
                tenantFeatures = this.modulesAgent.GetFeatures(tenantName).ToArray();
                this.features.Add(tenantName, tenantFeatures);
            }

            return tenantFeatures.FirstOrDefault(
                f => f.Name.Equals(featureName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}