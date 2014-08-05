namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Management.Automation;
    using System.Text.RegularExpressions;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    /// <summary>
    /// Implements the <c>New-Tenant</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class NewTenant : OrchardCmdlet
    {
        private IShellSettingsManager shellSettingsManager;

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenant's request URL host.
        /// </summary>
        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 2)]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design")]
        public string RequestUrlHost { get; set; }

        /// <summary>
        /// Gets or sets the tenant's request URL prefix.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design")]
        public string RequestUrlPrefix { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the tenant's database.
        /// </summary>
        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the data provider used by the tenant.
        /// </summary>
        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string DataProvider { get; set; }

        /// <summary>
        /// Gets or sets the tenant's data table prefix.
        /// </summary>
        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string DataTablePrefix { get; set; }

        /// <summary>
        /// Gets or sets the tenant to enable.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true)]
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
                var tenant = new ShellSettings();
                PropertyMapper.Instance.MapProperties(this, tenant);

                this.InvokeCreateTenant(tenant);
            }
            else if (this.ParameterSetName == "TenantObject")
            {
                this.InvokeCreateTenant(this.Tenant);
            }
        }

        /// <summary>
        /// Invokes the tenants agent to create a new tenant.
        /// </summary>
        /// <param name="tenant">The tenant to create.</param>
        private void InvokeCreateTenant(ShellSettings tenant)
        {
            if (this.ShouldProcess("Tenant: " + tenant.Name, "Create"))
            {
                try
                {
                    if (!string.IsNullOrEmpty(tenant.Name) && !Regex.IsMatch(tenant.Name, @"^\w+$"))
                    {
                        var exception = new InvalidOperationException(
                            "Invalid tenant name. Must contain characters only and no spaces.");

                        this.WriteError(exception, "CannotCreateTenant", ErrorCategory.InvalidArgument);
                        return;
                    }

                    if (tenant.Name == ShellSettings.DefaultName)
                    {
                        var exception = new InvalidOperationException("Invalid tenant name.");
                        this.WriteError(exception, "CannotCreateTenant", ErrorCategory.InvalidArgument);
                        return;
                    }

                    ShellSettings defaultTenant = this.shellSettingsManager.LoadSettings().FirstOrDefault(
                        x => x.Name == ShellSettings.DefaultName);

                    if (defaultTenant == null)
                    {
                        var exception = new InvalidOperationException("Failed to find default tenant.");
                        this.WriteError(exception, "FailedToFindDefaultTenant", ErrorCategory.ObjectNotFound);
                        return;
                    }

                    tenant.State = TenantState.Uninitialized;
                    tenant.Themes = defaultTenant.Themes;
                    tenant.Modules = defaultTenant.Modules;

                    this.shellSettingsManager.SaveSettings(tenant);
                }
                catch (Exception ex)
                {
                    this.WriteError(ex, "FailedToCreateTenant", ErrorCategory.NotSpecified);
                }
            }
        }
    }
}