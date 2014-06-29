// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetOrchardTheme.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Tenants.Items;

    /// <summary>
    /// Implements the <c>Get-OrchardTheme</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OrchardTheme", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardTheme : OrchardCmdlet
    {
        /// <summary>
        /// The modules agent instance.
        /// </summary>
        private IModulesAgent modulesAgent;

        /// <summary>
        /// All available Orchard tenants.
        /// </summary>
        private OrchardTenant[] allTenants;

        /// <summary>
        /// Gets or sets the name of the Orchard theme to get.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which themes will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which themes will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public OrchardTenant TenantObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether themes should be retrieved from all tenants.
        /// </summary>
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public SwitchParameter FromAllTenants { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only enabled themes should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only disabled themes should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Disabled { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.modulesAgent = this.AgentManager.GetAgent<IModulesAgent>();

            this.allTenants = this.AgentManager.GetAgent<ITenantAgent>()
                .GetTenants()
                .Where(t => t.State == TenantState.Running)
                .ToArray();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            IEnumerable<OrchardTenant> tenants = this.GetTenantsFromParameters();

            var themes = new List<OrchardTheme>();

            foreach (OrchardTenant tenant in tenants)
            {
                OrchardTheme[] tenantThemes = this.modulesAgent.GetThemes(tenant.Name);
                themes.AddRange(tenantThemes);
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                themes = themes.Where(f => f.Name.WildcardEquals(this.Name)).ToList();
            }

            if (this.Enabled.ToBool())
            {
                themes = themes.Where(f => f.Enabled).ToList();
            }

            if (this.Disabled.ToBool())
            {
                themes = themes.Where(f => !f.Enabled).ToList();
            }

            foreach (OrchardTheme theme in themes)
            {
                this.WriteObject(theme);
            }
        }

        /// <summary>
        /// Gets a list of Orchard tenants from which themes should be returned.
        /// </summary>
        /// <returns>A sequence of <see cref="OrchardTenant"/> objects.</returns>
        private IEnumerable<OrchardTenant> GetTenantsFromParameters()
        {
            var tenants = new List<OrchardTenant>();

            if (this.FromAllTenants.ToBool())
            {
                tenants.AddRange(this.allTenants);
            }
            else if (!string.IsNullOrEmpty(this.Tenant))
            {
                OrchardTenant namedTenant = this.allTenants.SingleOrDefault(
                    s => s.Name.Equals(this.Tenant, StringComparison.OrdinalIgnoreCase));

                if (namedTenant != null)
                {
                    tenants.Add(namedTenant);
                }
                else
                {
                    var ex = new ArgumentException("Failed to find tenant with name '" + this.Tenant + "'.");
                    this.WriteError(ex, "FailedToFindTenant", ErrorCategory.InvalidArgument);
                }
            }
            else if (this.TenantObject != null)
            {
                tenants.Add(this.TenantObject);
            }
            else
            {
                OrchardTenant tenant = this.GetCurrentTenant();
                if (tenant != null)
                {
                    tenants.Add(tenant);
                }
                else
                {
                    OrchardTenant defaultTenant = this.allTenants.SingleOrDefault(
                        s => s.Name.Equals("Default", StringComparison.OrdinalIgnoreCase));

                    if (defaultTenant != null)
                    {
                        tenants.Add(defaultTenant);
                    }
                    else
                    {
                        var ex = new ArgumentException("Failed to find tenant with name 'Default'.");
                        this.WriteError(ex, "FailedToFindTenant", ErrorCategory.InvalidArgument);
                    }
                }
            }

            return tenants;
        }
    }
}