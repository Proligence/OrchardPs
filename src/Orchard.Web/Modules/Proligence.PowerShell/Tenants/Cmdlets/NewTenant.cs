namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;

    using Orchard.Environment.Configuration;

    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements the <c>New-Tenant</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class NewTenant : OrchardCmdlet
    {
        /// <summary>
        /// The tenant agent proxy instance.
        /// </summary>
        private ITenantAgent tenantAgent;

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
                    this.tenantAgent.CreateTenant(tenant);
                }
                catch (ArgumentException ex)
                {
                    this.WriteError(ex, "CannotCreateTenant", ErrorCategory.InvalidArgument);
                }
                catch (InvalidOperationException ex)
                {
                    this.WriteError(ex, "CannotCreateTenant", ErrorCategory.InvalidOperation);
                }
            }
        }
    }
}