// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableOrchardFeature.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Sites.Items;

    /// <summary>
    /// Implements the <c>Disable-OrchardFeature</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Disable, "OrchardFeature", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class DisableOrchardFeature : OrchardCmdlet
    {
        /// <summary>
        /// The modules agent proxy instance.
        /// </summary>
        private ModulesAgentProxy modulesAgent;

        /// <summary>
        /// Cached list of all Orchard features for each site.
        /// </summary>
        private IDictionary<string, OrchardFeature[]> features;

        /// <summary>
        /// Gets or sets the name of the feature to disable.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the site for which the feature will be disabled.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Site { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OrchardFeature"/> object which represents the orchard feature to disable.
        /// </summary>
        [Parameter(ParameterSetName = "FeatureObject", ValueFromPipeline = true)]
        public OrchardFeature Feature { get; set; }

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
            this.modulesAgent = this.AgentManager.GetAgent<ModulesAgentProxy>();
            this.features = new Dictionary<string, OrchardFeature[]>();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            string siteName = null;

            if ((this.ParameterSetName != "FeatureObject") && string.IsNullOrEmpty(this.Site))
            {
                // Get feature for current site if site name not specified
                OrchardSite site = this.GetCurrentSite();
                siteName = site != null ? site.Name : "Default";
            }
            
            if (!string.IsNullOrEmpty(this.Site))
            {
                siteName = this.Site;
            }

            OrchardFeature feature = null;

            if (this.ParameterSetName == "Default")
            {
                feature = GetOrchardFeature(siteName, this.Name);
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
                siteName = this.Feature.SiteName;
            }

            if (feature != null)
            {
                if (this.ShouldProcess("Feature: " + feature.Id + ", Site: " + siteName, "Disable Feature"))
                {
                    this.modulesAgent.DisableFeature(siteName, feature.Id, !this.WithoutDependencies.ToBool());
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="OrchardFeature"/> object for the specified feature.
        /// </summary>
        /// <param name="siteName">The name ot the site for which to get feature.</param>
        /// <param name="featureName">The name of the feature to get.</param>
        /// <returns>The <see cref="OrchardFeature"/> object for the specified feature.</returns>
        private OrchardFeature GetOrchardFeature(string siteName, string featureName)
        {
            OrchardFeature[] siteFeatures;

            if (!this.features.TryGetValue(siteName, out siteFeatures))
            {
                siteFeatures = this.modulesAgent.GetFeatures(siteName).ToArray();
                this.features.Add(siteName, siteFeatures);
            }

            return siteFeatures.FirstOrDefault(
                f => f.Name.Equals(featureName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}