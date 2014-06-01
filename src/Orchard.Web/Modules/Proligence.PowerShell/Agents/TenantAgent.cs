// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantAgent.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Tenants.Items;

    /// <summary>
    /// Implements the the agent which exposes Orchard tenants.
    /// </summary>
    public class TenantAgent : AgentBase, ITenantAgent
    {
        /// <summary>
        /// Gets the tenants configured in the Orchard installation.
        /// </summary>
        /// <returns>
        /// An array of <see cref="OrchardTenant"/> objects which represent the tenants configured in the Orchard
        /// installation.
        /// </returns>
        public OrchardTenant[] GetTenants() 
        {
            var tenantManager = HostContainer.Resolve<IShellSettingsManager>();
            IEnumerable<ShellSettings> settings = tenantManager.LoadSettings();
            IEnumerable<OrchardTenant> tenants = settings.Select(ShellSettingsToOrchardTenant);

            return tenants.ToArray();
        }

        /// <summary>
        /// Gets the tenant with the specified name.
        /// </summary>
        /// <returns>
        /// A <see cref="OrchardTenant"/> object which represent the tenant with the specified name or <c>null</c>.
        /// </returns>
        public OrchardTenant GetTenant(string name)
        {
            var tenantManager = HostContainer.Resolve<IShellSettingsManager>();

            return tenantManager
                .LoadSettings()
                .Where(t => t.Name == name)
                .Select(ShellSettingsToOrchardTenant)
                .FirstOrDefault();
        }

        /// <summary>
        /// Enables the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to enable.</param>
        public void EnableTenant(string name)
        {
            var shellSettingsManager = HostContainer.Resolve<IShellSettingsManager>();
            ShellSettings tenant = shellSettingsManager.LoadSettings().FirstOrDefault(x => x.Name == name);
            if (tenant == null)
            {
                throw new ArgumentException("Failed to find tenant '" + name + "'.");
            }

            if (tenant.Name == ShellSettings.DefaultName)
            {
                throw new InvalidOperationException("Cannot enable default tenant.");
            }

            if (tenant.State != TenantState.Running)
            {
                tenant.State = TenantState.Running;
                shellSettingsManager.SaveSettings(tenant);
            }
        }

        /// <summary>
        /// Disables the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to disable.</param>
        public void DisableTenant(string name)
        {
            var shellSettingsManager = HostContainer.Resolve<IShellSettingsManager>();
            ShellSettings tenant = shellSettingsManager.LoadSettings().FirstOrDefault(x => x.Name == name);
            if (tenant == null)
            {
                throw new ArgumentException("Failed to find tenant '" + name + "'.");
            }

            if (tenant.Name == ShellSettings.DefaultName)
            {
                throw new InvalidOperationException("Cannot disable default tenant.");
            }

            if (tenant.State != TenantState.Disabled)
            {
                tenant.State = TenantState.Disabled;
                shellSettingsManager.SaveSettings(tenant);
            }
        }

        /// <summary>
        /// Creates a <see cref="OrchardTenant"/> object from a <see cref="ShellSettings"/> object.
        /// </summary>
        /// <param name="shellSettings">The tenant's shell settings.</param>
        /// <returns>The created <see cref="OrchardTenant"/> object.</returns>
        private static OrchardTenant ShellSettingsToOrchardTenant(ShellSettings shellSettings)
        {
            return new OrchardTenant
            {
                Name = shellSettings.Name,
                State = shellSettings.State,
                DataConnectionString = shellSettings.DataConnectionString,
                DataProvider = shellSettings.DataProvider,
                DataTablePrefix = shellSettings.DataTablePrefix,
                EncryptionAlgorithm = shellSettings.EncryptionAlgorithm,
                EncryptionKey = shellSettings.EncryptionKey,
                HashAlgorithm = shellSettings.HashAlgorithm,
                HashKey = shellSettings.HashKey,
                RequestUrlHost = shellSettings.RequestUrlHost,
                RequestUrlPrefix = shellSettings.RequestUrlPrefix
            };
        }
    }
}