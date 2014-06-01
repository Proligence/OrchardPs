// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITenantAgent.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents
{
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
        /// Enables the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to enable.</param>
        void EnableTenant(string name);
    }
}