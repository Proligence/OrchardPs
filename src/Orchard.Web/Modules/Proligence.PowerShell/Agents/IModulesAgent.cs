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
        /// Gets all modules for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        OrchardModule[] GetModules(string tenant);

        /// <summary>
        /// Gets all features for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        OrchardFeature[] GetFeatures(string tenant);

        /// <summary>
        /// Enables the specified feature.
        /// </summary>
        /// <param name="tenant">The name of the tenant for which the feature will be enabled.</param>
        /// <param name="name">The name of the feature to enable.</param>
        /// <param name="includeDependencies">True to enable dependant features; otherwise, false.</param>
        void EnableFeature(string tenant, string name, bool includeDependencies);

        /// <summary>
        /// Disables the specified feature.
        /// </summary>
        /// <param name="tenant">The name of the tenant for which the feature will be disabled.</param>
        /// <param name="name">The name of the feature to disable.</param>
        /// <param name="includeDependencies">True to disable dependant features; otherwise, false.</param>
        void DisableFeature(string tenant, string name, bool includeDependencies);

        /// <summary>
        /// Gets all themes for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>An array of objects representing themes from the specified tenant.</returns>
        OrchardTheme[] GetThemes(string tenant);

        /// <summary>
        /// Enables the specified theme for a tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant for which the theme will be enabled.</param>
        /// <param name="id">The identifier of the theme to enable.</param>
        void EnableTheme(string tenant, string id);
    }
}