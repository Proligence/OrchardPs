// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableTenant.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System;
    using System.Management.Automation;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Tenants.Items;

    /// <summary>
    /// Implements the <c>Enable-Tenant</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Enable, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EnableTenant : OrchardCmdlet
    {
        /// <summary>
        /// The tenant agent proxy instance.
        /// </summary>
        private ITenantAgent tenantAgent;

        /// <summary>
        /// Gets or sets the name of the tenant to enable.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenant to enable.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", ValueFromPipeline = true)]
        public OrchardTenant Tenant { get; set; }

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
            if (this.ParameterSetName == "Default")
            {
                this.InvokeEnableTenant(this.Name);
            }
            else if (this.ParameterSetName == "TenantObject")
            {
                this.InvokeEnableTenant(this.Tenant.Name);
            }
        }

        /// <summary>
        /// Invokes the operation to enable a tenant.
        /// </summary>
        /// <param name="name">The name of the tenant to enable.</param>
        private void InvokeEnableTenant(string name)
        {
            if (this.ShouldProcess("Tenant: " + name, "Enable Tenant"))
            {
                try
                {
                    this.tenantAgent.EnableTenant(name);
                }
                catch (ArgumentException ex)
                {
                    this.WriteError(ex, "FailedToFindTenant", ErrorCategory.ObjectNotFound, name);
                }
                catch (InvalidOperationException ex)
                {
                    this.WriteError(ex, "CannotEnableDefaultTenant", ErrorCategory.InvalidOperation, name);
                }
            }
        }
    }
}