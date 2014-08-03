namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements the <c>Disable-Tenant</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Disable, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class DisableTenant : OrchardCmdlet
    {
        private IShellSettingsManager shellSettingsManager;

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
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.shellSettingsManager = this.OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
        }

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

        private void InvokeDisableTenant(string name)
        {
            if (this.ShouldProcess("Tenant: " + name, "Disable Tenant"))
            {
                try
                {
                    ShellSettings tenant = this.shellSettingsManager.LoadSettings()
                        .FirstOrDefault(x => x.Name == name);

                    if (tenant == null)
                    {
                        var ex = new ArgumentException("Failed to find tenant '" + name + "'.");
                        this.WriteError(ex, "FailedToFindTenant", ErrorCategory.ObjectNotFound, name);
                        return;
                    }

                    if (tenant.Name == ShellSettings.DefaultName)
                    {
                        var ex = new InvalidOperationException("Cannot disable default tenant.");
                        this.WriteError(ex, "CannotDisableDefaultTenant", ErrorCategory.InvalidOperation, name);
                        return;
                    }

                    if (tenant.State != TenantState.Disabled)
                    {
                        tenant.State = TenantState.Disabled;
                        this.shellSettingsManager.SaveSettings(tenant);
                    }
                }
                catch (Exception ex)
                {
                    this.WriteError(ex, "FailedToDisableTenant", ErrorCategory.NotSpecified, name);
                }
            }
        }
    }
}