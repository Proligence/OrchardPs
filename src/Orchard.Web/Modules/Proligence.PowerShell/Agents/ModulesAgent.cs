﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModulesAgent.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents
{
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Modules.Items;

    /// <summary>
    /// Implements the agent which exposes Orchard modules and features.
    /// </summary>
    public class ModulesAgent : AgentBase
    {
        /// <summary>
        /// Gets all modules for the specified site.
        /// </summary>
        /// <param name="site">The name of the site which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        public OrchardModule[] GetModules(string site)
        {
            IExtensionManager extensionManager = this.GetExtensionManager(site);
            IEnumerable<ExtensionDescriptor> extensionDescriptors = extensionManager.AvailableExtensions();

            OrchardModule[] modules = extensionDescriptors.Select(MapDescriptorToOrchardModule).ToArray();
            foreach (OrchardModule orchardModule in modules)
            {
                foreach (OrchardFeature orchardFeature in orchardModule.Features)
                {
                    orchardFeature.Module = orchardModule;
                }
            }

            return modules;
        }

        /// <summary>
        /// Gets all features for the specified site.
        /// </summary>
        /// <param name="site">The name of the site which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        public OrchardFeature[] GetFeatures(string site)
        {
            IExtensionManager extensionManager = this.GetExtensionManager(site);
            IEnumerable<FeatureDescriptor> featureDescriptors = extensionManager.AvailableFeatures();
            IEnumerable<ExtensionDescriptor> extensions = extensionManager.AvailableExtensions().ToArray();

            var features = new List<OrchardFeature>();

            foreach (FeatureDescriptor featureDescriptor in featureDescriptors)
            {
                OrchardFeature feature = MapDescriptorToOrchardFeature(featureDescriptor);
                
                ExtensionDescriptor extension = extensions.FirstOrDefault(e => e.Name == feature.Name);
                if (extension != null)
                {
                    feature.Module = MapDescriptorToOrchardModule(extension);
                }

                features.Add(feature);
            }

            return features.ToArray();
        }

        /// <summary>
        /// Gets the <see cref="IExtensionManager"/> instance for the specified site.
        /// </summary>
        /// <param name="site">The name of the site.</param>
        /// <returns>The <see cref="IExtensionManager"/> instance for the specified site.</returns>
        private IExtensionManager GetExtensionManager(string site)
        {
            ILifetimeScope siteContainer = this.ContainerManager.GetSiteContainer(site);
            return siteContainer.Resolve<IExtensionManager>();
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