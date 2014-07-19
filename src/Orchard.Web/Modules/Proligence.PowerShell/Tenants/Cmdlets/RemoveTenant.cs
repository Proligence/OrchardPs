namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System;
    using System.Management.Automation;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Tenants.Items;

    /// <summary>
    /// Implements the <c>Remove-Tenant</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTenant : OrchardCmdlet
    {
        /// <summary>
        /// The tenant agent proxy instance.
        /// </summary>
        private ITenantAgent tenantAgent;

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenant to enable.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
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
                this.InvokeRemoveTenant(this.Name);
            }
            else if (this.ParameterSetName == "TenantObject")
            {
                this.InvokeRemoveTenant(this.Tenant.Name);
            }
        }

        /// <summary>
        /// Invokes the tenants agent to remove an existing tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant to remove.</param>
        private void InvokeRemoveTenant(string tenantName)
        {
            if (this.ShouldProcess("Tenant: " + tenantName, "Remove"))
            {
                try
                {
                    this.tenantAgent.RemoveTenant(tenantName);
                }
                catch (ArgumentException ex)
                {
                    this.WriteError(ex, "CannotRemoveTenant", ErrorCategory.InvalidArgument);
                }
                catch (InvalidOperationException ex)
                {
                    this.WriteError(ex, "CannotRemoveTenant", ErrorCategory.InvalidOperation);
                }
            }
        }
    }
}