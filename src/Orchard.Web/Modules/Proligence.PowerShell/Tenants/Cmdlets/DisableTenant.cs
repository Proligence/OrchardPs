namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System;
    using System.Management.Automation;

    using Orchard.Environment.Configuration;

    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements the <c>Disable-Tenant</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Disable, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class DisableTenant : OrchardCmdlet
    {
        /// <summary>
        /// The tenant agent proxy instance.
        /// </summary>
        private ITenantAgent tenantAgent;

        /// <summary>
        /// Gets or sets the name of the tenant to disable.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenant to disable.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", ValueFromPipeline = true)]
        public ShellSettings Tenant { get; set; }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "Default")
            {
                this.InvokeDisableTenant(this.Name);
            }
            else if (this.ParameterSetName == "TenantObject")
            {
                this.InvokeDisableTenant(this.Tenant.Name);
            }
        }

        /// <summary>
        /// Invokes the operation to disable a tenant.
        /// </summary>
        /// <param name="name">The name of the tenant to disable.</param>
        private void InvokeDisableTenant(string name)
        {
            if (this.ShouldProcess("Tenant: " + name, "Disable Tenant"))
            {
                try
                {
                    this.tenantAgent.DisableTenant(name);
                }
                catch (ArgumentException ex)
                {
                    this.WriteError(ex, "FailedToFindTenant", ErrorCategory.ObjectNotFound, name);
                }
                catch (InvalidOperationException ex)
                {
                    this.WriteError(ex, "CannotDisableDefaultTenant", ErrorCategory.InvalidOperation, name);
                }
            }
        }
    }
}