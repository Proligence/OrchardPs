// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantAgentProxy.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Sites.Items;

    /// <summary>
    /// Implements the proxy for the agent which exposes Orchard sites (tenants).
    /// </summary>
    [Agent("Proligence.PowerShell.Agents.TenantAgent, Proligence.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class TenantAgentProxy : AgentProxy 
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
            return (OrchardSite[])Invoke("GetSites");
        }
    }
}