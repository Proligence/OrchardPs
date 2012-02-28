// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantAgent.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents 
{
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Sites.Items;

    /// <summary>
    /// Implements the the agent which exposes Orchard sites (tenants).
    /// </summary>
    public class TenantAgent : AgentBase 
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
                    State = s.State.CurrentState,
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
    }
}