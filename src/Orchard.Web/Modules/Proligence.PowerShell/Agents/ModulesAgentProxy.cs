// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModulesAgent.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Modules.Items;

    /// <summary>
    /// Implements the proxy for the agent which exposes Orchard modules and features.
    /// </summary>
    [Agent("Proligence.PowerShell.Agents.ModulesAgent, Proligence.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class ModulesAgentProxy : AgentProxy 
    {
        /// <summary>
        /// Gets all modules for the specified site.
        /// </summary>
        /// <param name="site">The name of the site which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        public OrchardModule[] GetModules(string site)
        {
            return (OrchardModule[])Invoke("GetModules", site);
        }

        /// <summary>
        /// Gets all features for the specified site.
        /// </summary>
        /// <param name="site">The name of the site which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        public OrchardFeature[] GetFeatures(string site)
        {
            return (OrchardFeature[])Invoke("GetFeatures", site);
        }

        /// <summary>
        /// Enables the specified feature.
        /// </summary>
        /// <param name="site">The name of the site for which the feature will be enabled.</param>
        /// <param name="name">The name of the feature to enable.</param>
        /// <param name="includeDependencies">True to enable dependant features; otherwise, false.</param>
        public void EnableFeature(string site, string name, bool includeDependencies)
        {
            this.Invoke("EnableFeature", site, name, includeDependencies);
        }
    }
}