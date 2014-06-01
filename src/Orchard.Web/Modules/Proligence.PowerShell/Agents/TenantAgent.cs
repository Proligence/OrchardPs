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
    using Proligence.PowerShell.Sites.Items;

    /// <summary>
    /// Implements the the agent which exposes Orchard sites (tenants).
    /// </summary>
    public class TenantAgent : AgentBase, ITenantAgent
    {
        /// <summary>
        /// Gets sites (tenants) configured in the Orchard installation.
        /// </summary>
        /// <returns>
        /// An array of <see cref="OrchardSite"/> objects which represent the sites configured in the Orchard
        /// installation.
        /// </returns>
        public OrchardSite[] GetSites() 
        {
            IShellSettingsManager tenantManager = HostContainer.Resolve<IShellSettingsManager>();
            IEnumerable<ShellSettings> settings = tenantManager.LoadSettings();
            IEnumerable<OrchardSite> sites = settings.Select(
                s => new OrchardSite 
                {
                    Name = s.Name,
                    State = s.State,
                    DataConnectionString = s.DataConnectionString,
                    DataProvider = s.DataProvider,
                    DataTablePrefix = s.DataTablePrefix,
                    EncryptionAlgorithm = s.EncryptionAlgorithm,
                    EncryptionKey = s.EncryptionKey,
                    HashAlgorithm = s.HashAlgorithm,
                    HashKey = s.HashKey,
                    RequestUrlHost = s.RequestUrlHost,
                    RequestUrlPrefix = s.RequestUrlPrefix
                });

            return sites.ToArray();
        }

        /// <summary>
        /// Enables the site (tenant) with the specified name.
        /// </summary>
        /// <param name="name">The name of the site to enable.</param>
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

            tenant.State = TenantState.Running;
            shellSettingsManager.SaveSettings(tenant);
        }
    }
}