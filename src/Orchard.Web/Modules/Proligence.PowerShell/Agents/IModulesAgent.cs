// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModulesAgent.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Modules.Items;

    /// <summary>
    /// Defines the interfaces for the agent which exposes Orchard modules and features.
    /// </summary>
    [Agent(typeof(ModulesAgent))]
    public interface IModulesAgent : IAgent
    {
        /// <summary>
        /// Gets all modules for the specified site.
        /// </summary>
        /// <param name="site">The name of the site which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        OrchardModule[] GetModules(string site);

        /// <summary>
        /// Gets all features for the specified site.
        /// </summary>
        /// <param name="site">The name of the site which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        OrchardFeature[] GetFeatures(string site);

        /// <summary>
        /// Enables the specified feature.
        /// </summary>
        /// <param name="site">The name of the site for which the feature will be enabled.</param>
        /// <param name="name">The name of the feature to enable.</param>
        /// <param name="includeDependencies">True to enable dependant features; otherwise, false.</param>
        void EnableFeature(string site, string name, bool includeDependencies);

        /// <summary>
        /// Disables the specified feature.
        /// </summary>
        /// <param name="site">The name of the site for which the feature will be disabled.</param>
        /// <param name="name">The name of the feature to disable.</param>
        /// <param name="includeDependencies">True to disable dependant features; otherwise, false.</param>
        void DisableFeature(string site, string name, bool includeDependencies);
    }
}