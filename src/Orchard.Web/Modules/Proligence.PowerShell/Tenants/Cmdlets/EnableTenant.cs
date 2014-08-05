namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;

    [Cmdlet(VerbsLifecycle.Enable, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EnableTenant : OrchardCmdlet
    {
        private IShellSettingsManager shellSettingsManager;

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
        public ShellSettings Tenant { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.shellSettingsManager = this.OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
        }

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

        private void InvokeEnableTenant(string name)
        {
            if (this.ShouldProcess("Tenant: " + name, "Enable Tenant"))
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
                        var ex = new InvalidOperationException("Cannot enable default tenant.");
                        this.WriteError(ex, "CannotEnableDefaultTenant", ErrorCategory.InvalidOperation, name);
                        return;
                    }

                    if (tenant.State != TenantState.Running)
                    {
                        tenant.State = TenantState.Running;
                        this.shellSettingsManager.SaveSettings(tenant);
                    }
                }
                catch (Exception ex)
                {
                    this.WriteError(ex, "FailedToEnableTenant", ErrorCategory.NotSpecified, name);
                }
            }
        }
    }
}