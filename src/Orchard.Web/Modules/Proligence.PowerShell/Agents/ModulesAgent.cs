// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModulesAgent.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Orchard.Data.Migration;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Environment.Features;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Modules.Items;

    /// <summary>
    /// Implements the agent which exposes Orchard modules and features.
    /// </summary>
    public class ModulesAgent : AgentBase, IModulesAgent
    {
        /// <summary>
        /// Gets all modules for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        public OrchardModule[] GetModules(string tenant)
        {
            IExtensionManager extensionManager = this.GetExtensionManager(tenant);
            IFeatureManager featureManager = this.GetFeatureManager(tenant);
            IEnumerable<ExtensionDescriptor> extensionDescriptors = extensionManager.AvailableExtensions();
            IEnumerable<FeatureDescriptor> enabledFeatures = featureManager.GetEnabledFeatures().ToArray();

            OrchardModule[] modules = extensionDescriptors.Select(MapDescriptorToOrchardModule).ToArray();
            foreach (OrchardModule orchardModule in modules)
            {
                foreach (OrchardFeature orchardFeature in orchardModule.Features)
                {
                    orchardFeature.Module = orchardModule;
                    orchardFeature.Enabled = enabledFeatures.Any(f => f.Name == orchardFeature.Name);
                    orchardFeature.TenantName = tenant;
                }
            }

            return modules;
        }

        /// <summary>
        /// Gets all features for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        public OrchardFeature[] GetFeatures(string tenant)
        {
            IExtensionManager extensionManager = this.GetExtensionManager(tenant);
            IFeatureManager featureManager = this.GetFeatureManager(tenant);
            IEnumerable<FeatureDescriptor> featureDescriptors = extensionManager.AvailableFeatures();
            IEnumerable<ExtensionDescriptor> extensions = extensionManager.AvailableExtensions().ToArray();
            IEnumerable<FeatureDescriptor> enabledFeatures = featureManager.GetEnabledFeatures().ToArray();

            var features = new List<OrchardFeature>();

            foreach (FeatureDescriptor featureDescriptor in featureDescriptors)
            {
                OrchardFeature feature = MapDescriptorToOrchardFeature(featureDescriptor);
                
                ExtensionDescriptor extension = extensions.FirstOrDefault(e => e.Name == feature.Name);
                if (extension != null)
                {
                    feature.Module = MapDescriptorToOrchardModule(extension);
                    feature.Enabled = enabledFeatures.Any(f => f.Name == feature.Name);
                }

                features.Add(feature);
            }

            foreach (OrchardFeature orchardFeature in features)
            {
                orchardFeature.TenantName = tenant;
            }

            return features.ToArray();
        }

        /// <summary>
        /// Enables the specified feature.
        /// </summary>
        /// <param name="tenant">The name of the tenant for which the feature will be enabled.</param>
        /// <param name="name">The name of the feature to enable.</param>
        /// <param name="includeDependencies">True to enable dependant features; otherwise, false.</param>
        public void EnableFeature(string tenant, string name, bool includeDependencies)
        {
            IFeatureManager featureManager = this.GetFeatureManager(tenant);
            featureManager.EnableFeatures(new[] { name }, includeDependencies);
        }

        /// <summary>
        /// Disables the specified feature.
        /// </summary>
        /// <param name="tenant">The name of the tenant for which the feature will be disabled.</param>
        /// <param name="name">The name of the feature to disable.</param>
        /// <param name="includeDependencies">True to disable dependant features; otherwise, false.</param>
        public void DisableFeature(string tenant, string name, bool includeDependencies)
        {
            IFeatureManager featureManager = this.GetFeatureManager(tenant);
            featureManager.DisableFeatures(new[] { name }, includeDependencies);
        }

        /// <summary>
        /// Gets all themes for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>An array of objects representing themes from the specified tenant.</returns>
        public OrchardTheme[] GetThemes(string tenant)
        {
            IDataMigrationManager dataMigrationManager = this.GetDataMigrationManager(tenant);
            IEnumerable<string> featuresThatNeedUpdate = dataMigrationManager.GetFeaturesThatNeedUpdate();

            IExtensionManager extensionManager = this.GetExtensionManager(tenant);
            
            return extensionManager.AvailableExtensions()
                .Where(d => DefaultExtensionTypes.IsTheme(d.ExtensionType))
                .Where(d => d.Tags != null && d.Tags.Split(',').Any(
                    t => t.Trim().Equals("hidden", StringComparison.OrdinalIgnoreCase)) == false)
                .Select(
                    d => new OrchardTheme
                    {
                        Module = MapDescriptorToOrchardModule(d),
                        Enabled = this.GetFeatures(tenant).Any(f => f.Id == d.Id && f.Enabled),
                        NeedsUpdate = featuresThatNeedUpdate.Contains(d.Id)
                    })
                .ToArray();
        }

        /// <summary>
        /// Gets the <see cref="IExtensionManager"/> instance for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>The <see cref="IExtensionManager"/> instance for the specified tenant.</returns>
        private IExtensionManager GetExtensionManager(string tenant)
        {
            ILifetimeScope tenantContainer = this.ContainerManager.GetTenantContainer(tenant);
            return tenantContainer.Resolve<IExtensionManager>();
        }

        /// <summary>
        /// Gets the <see cref="IFeatureManager"/> instance for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>The <see cref="IFeatureManager"/> instance for the specified tenant.</returns>
        private IFeatureManager GetFeatureManager(string tenant)
        {
            ILifetimeScope tenantContainer = this.ContainerManager.GetTenantContainer(tenant);
            return tenantContainer.Resolve<IFeatureManager>();
        }

        /// <summary>
        /// Gets the <see cref="IDataMigrationManager"/> instance for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>The <see cref="IDataMigrationManager"/> instance for the specified tenant.</returns>
        private IDataMigrationManager GetDataMigrationManager(string tenant)
        {
            ILifetimeScope tenantContainer = this.ContainerManager.GetTenantContainer(tenant);
            return tenantContainer.Resolve<IDataMigrationManager>();
        }

        /// <summary>
        /// Maps an orchard module descriptor to a new <see cref="OrchardModule"/> object.
        /// </summary>
        /// <param name="descriptor">The descriptor to map.</param>
        /// <returns>The created object.</returns>
        private static OrchardModule MapDescriptorToOrchardModule(ExtensionDescriptor descriptor)
        {
            return new OrchardModule
                   {
                       Id = descriptor.Id,
                       ExtensionType = descriptor.ExtensionType,
                       Name = descriptor.Name,
                       Path = descriptor.Path,
                       Description = descriptor.Description,
                       Version = descriptor.Version,
                       OrchardVersion = descriptor.OrchardVersion,
                       Author = descriptor.Author,
                       Website = descriptor.WebSite,
                       Tags = descriptor.Tags,
                       AntiForgery = descriptor.AntiForgery,
                       Zones = descriptor.Zones,
                       BaseTheme = descriptor.BaseTheme,
                       SessionState = descriptor.SessionState,
                       Features = descriptor.Features.Select(MapDescriptorToOrchardFeature).ToArray()
                   };
        }

        /// <summary>
        /// Maps an orchard module descriptor to a new <see cref="OrchardFeature"/> object.
        /// </summary>
        /// <param name="descriptor">The descriptor to map.</param>
        /// <returns>The created object.</returns>
        private static OrchardFeature MapDescriptorToOrchardFeature(FeatureDescriptor descriptor)
        {
            return new OrchardFeature
                   {
                       Id = descriptor.Id,
                       Name = descriptor.Name,
                       Description = descriptor.Description,
                       Category = descriptor.Category,
                       Priority = descriptor.Priority,
                       Dependencies = descriptor.Dependencies != null ? descriptor.Dependencies.ToArray() : null
                   };
        }
    }
}