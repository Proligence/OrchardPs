namespace Proligence.PowerShell.Core.Utilities
{
    using System.Management.Automation;
    using Orchard.Environment.Configuration;

    /// <summary>
    /// Defines the interfaces for cmdlets which support common parameters for filtering tenants on which the cmdlet
    /// will perform its operation.
    /// </summary>
    public interface ITenantFilterCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the tenant for which the feature will be retrieved.
        /// </summary>
        string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which features will be retrieved.
        /// </summary>
        ShellSettings TenantObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether features should be retrieved from all tenants.
        /// </summary>
        SwitchParameter FromAllTenants { get; set; }
    }
}