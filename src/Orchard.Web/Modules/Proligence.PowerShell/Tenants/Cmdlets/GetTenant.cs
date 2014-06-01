// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetTenant.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Tenants.Items;

    /// <summary>
    /// Implements the <c>Get-Tenant</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "Tenant", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetTenant : OrchardCmdlet
    {
        /// <summary>
        /// The tenant agent instance.
        /// </summary>
        private ITenantAgent tenantAgent;

        /// <summary>
        /// Gets or sets the name of the tenant to get.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only enabled tenants should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Enabled", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only disabled features should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Disabled", Mandatory = false)]
        public SwitchParameter Disabled { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.tenantAgent = this.AgentManager.GetAgent<ITenantAgent>();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            IEnumerable<OrchardTenant> tenants = this.GetTenantsFromParameters();
            foreach (OrchardTenant tenant in tenants)
            {
                this.WriteObject(tenant);
            }
        }

        /// <summary>
        /// Gets the orchard tenants which satisfy the current cmdlet parameters.
        /// </summary>
        /// <returns>Sequence of <see cref="OrchardTenant"/> objects.</returns>
        private IEnumerable<OrchardTenant> GetTenantsFromParameters()
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                OrchardTenant tenant = this.tenantAgent.GetTenant(this.Name);
                if (tenant != null)
                {
                    yield return tenant;
                }
            }
            else
            {
                IEnumerable<OrchardTenant> tenants = this.tenantAgent.GetTenants();

                if (this.Enabled.ToBool())
                {
                    tenants = tenants.Where(t => t.State == TenantState.Running);
                }

                if (this.Disabled.ToBool())
                {
                    tenants = tenants.Where(t => t.State == TenantState.Disabled);
                }

                foreach (OrchardTenant tenant in tenants)
                {
                    yield return tenant;
                }
            }
        }
    }
}