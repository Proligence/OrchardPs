namespace Proligence.PowerShell.Agents
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Tenants.Items;

    /// <summary>
    /// Defines the interface for the agent which exposes Orchard tenants.
    /// </summary>
    [Agent(typeof(TenantAgent))]
    public interface ITenantAgent : IAgent
    {
        /// <summary>
        /// Gets the tenants configured in the Orchard installation.
        /// </summary>
        /// <returns>
        /// An array of <see cref="OrchardTenant"/> objects which represent the tenants configured in the Orchard
        /// installation.
        /// </returns>
        OrchardTenant[] GetTenants();

        /// <summary>
        /// Gets the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to get.</param>
        /// <returns>
        /// A <see cref="OrchardTenant"/> object which represent the tenant with the specified name or <c>null</c>.
        /// </returns>
        OrchardTenant GetTenant(string name);

        /// <summary>
        /// Enables the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to enable.</param>
        void EnableTenant(string name);

        /// <summary>
        /// Disables the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to disable.</param>
        void DisableTenant(string name);

        /// <summary>
        /// Creates a new tenant.
        /// </summary>
        /// <param name="tenant">The new tenant to create.</param>
        void CreateTenant(OrchardTenant tenant);

        /// <summary>
        /// Updates an existing tenant.
        /// </summary>
        /// <param name="tenant">The updated tenant.</param>
        void UpdateTenant(OrchardTenant tenant);

        /// <summary>
        /// Removes an existing tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant to remove.</param>
        void RemoveTenant(string tenantName);

        /// <summary>
        /// Gets the names of all supported tenant settings.
        /// </summary>
        /// <returns>A sequence of setting names.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IEnumerable<string> GetTenantSettingNames();

        /// <summary>
        /// Gets the value of the specified tenant setting.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="settingName">The name of the setting to get.</param>
        /// <returns>The value of the tenant setting.</returns>
        object GetTenantSetting(string tenantName, string settingName);

        /// <summary>
        /// Updates the value of the specified tenant setting.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="settingName">The name of the setting to update.</param>
        /// <param name="value">The new value for the tenant setting.</param>
        void UpdateTenantSetting(string tenantName, string settingName, object value);
    }
}