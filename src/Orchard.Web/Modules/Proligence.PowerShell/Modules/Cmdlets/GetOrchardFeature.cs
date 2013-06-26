// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetOrchardFeature.cs" company="Proligence">
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
    /// Implements the <c>Get-OrchardFeature</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OrchardFeature", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardFeature : OrchardCmdlet
    {
        /// <summary>
        /// The modules agent instance.
        /// </summary>
        private IModulesAgent modulesAgent;

        /// <summary>
        /// All available Orchard sites.
        /// </summary>
        private OrchardSite[] allSites;

        /// <summary>
        /// Gets or sets the name of the Orchard feature to get.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "SiteObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllSites", Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the site for which the feature will be enabled.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = false)]        
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Site { get; set; }

        [Parameter(ParameterSetName = "SiteObject", Mandatory = true, ValueFromPipeline = true)]
        public OrchardSite SiteObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether features should be get from all tenants.
        /// </summary>
        [Parameter(ParameterSetName = "AllSites", Mandatory = true)]
        public SwitchParameter FromAllSites { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only enabled features should be get.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "SiteObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllSites", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only disabled features should be get.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "SiteObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllSites", Mandatory = false)]
        public SwitchParameter Disabled { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.modulesAgent = this.AgentManager.GetAgent<IModulesAgent>();
            this.allSites = this.AgentManager.GetAgent<ITenantAgent>().GetSites();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            IEnumerable<OrchardSite> sites = this.GetSitesFromParameters();

            var features = new List<OrchardFeature>();

            foreach (OrchardSite site in sites)
            {
                OrchardFeature[] siteFeatures = this.modulesAgent.GetFeatures(site.Name);
                features.AddRange(siteFeatures);
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                features = features.Where(f => f.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (this.Enabled.ToBool())
            {
                features = features.Where(f => f.Enabled).ToList();
            }

            if (this.Disabled.ToBool())
            {
                features = features.Where(f => !f.Enabled).ToList();
            }

            foreach (OrchardFeature feature in features)
            {
                this.WriteObject(feature);
            }
        }

        /// <summary>
        /// Gets a list of Orchard sites from which features should be returned.
        /// </summary>
        /// <returns>A sequence of <see cref="OrchardSite"/> objects.</returns>
        private IEnumerable<OrchardSite> GetSitesFromParameters()
        {
            var sites = new List<OrchardSite>();

            if (this.FromAllSites.ToBool())
            {
                sites.AddRange(this.allSites);
            }
            else if (!string.IsNullOrEmpty(this.Site))
            {
                OrchardSite namedSite = this.allSites.SingleOrDefault(
                    s => s.Name.Equals(this.Site, StringComparison.OrdinalIgnoreCase));

                if (namedSite != null)
                {
                    sites.Add(namedSite);
                }
                else
                {
                    var ex = new ArgumentException("Failed to find site with name '" + this.Site + "'.");
                    this.WriteError(ex, "FailedToFindSite", ErrorCategory.InvalidArgument);
                }
            }
            else if (this.SiteObject != null)
            {
                sites.Add(this.SiteObject);
            }
            else
            {
                OrchardSite site = this.GetCurrentSite();
                if (site != null)
                {
                    sites.Add(site);
                }
                else
                {
                    OrchardSite defaultSite = this.allSites.SingleOrDefault(
                        s => s.Name.Equals("Default", StringComparison.OrdinalIgnoreCase));

                    if (defaultSite != null)
                    {
                        sites.Add(defaultSite);
                    }
                    else
                    {
                        var ex = new ArgumentException("Failed to find site with name 'Default'.");
                        this.WriteError(ex, "FailedToFindSite", ErrorCategory.InvalidArgument);
                    }
                }
            }

            return sites;
        }
    }
}