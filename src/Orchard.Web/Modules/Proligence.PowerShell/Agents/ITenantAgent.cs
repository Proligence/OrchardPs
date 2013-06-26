// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITenantAgent.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Sites.Items;

    /// <summary>
    /// Defines the interface for the agent which exposes Orchard sites (tenants).
    /// </summary>
    [Agent(typeof(TenantAgent))]
    public interface ITenantAgent : IAgent
    {
        /// <summary>
        /// Gets sites (tenants) configured in the Orchard installation.
        /// </summary>
        /// <returns>
        /// An array of <see cref="OrchardSite"/> objects which represent the sites configured in the Orchard
        /// installation.
        /// </returns>
        OrchardSite[] GetSites();
    }
}