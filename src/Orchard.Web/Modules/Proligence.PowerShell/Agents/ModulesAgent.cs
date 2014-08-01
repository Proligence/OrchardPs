namespace Proligence.PowerShell.Agents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Orchard;
    using Orchard.ContentManagement;
    using Orchard.Data.Migration;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Environment.Features;
    using Orchard.Themes.Models;
    using Orchard.Themes.Services;
    using Proligence.PowerShell.Modules.Items;

    /// <summary>
    /// Implements the agent which exposes Orchard modules and features.
    /// </summary>
    public class ModulesAgent : IModulesAgent
    {
        private readonly IFeatureManager features;
        private readonly IExtensionManager extensions;
        private readonly IDataMigrationManager migrations;
        private readonly IOrchardServices services;

        private readonly IThemeService themes;

        private readonly ISiteThemeService siteThemes;

        public ModulesAgent(
            IFeatureManager features, 
            IExtensionManager extensions, 
            IDataMigrationManager migrations, 
            IOrchardServices services, 
            IThemeService themes, 
            ISiteThemeService siteThemes) {
            this.features = features;
            this.extensions = extensions;
            this.migrations = migrations;
            this.services = services;
            this.themes = themes;
            this.siteThemes = siteThemes;
        }

        /// <summary>
        /// Gets all modules for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        public ExtensionDescriptor[] GetModules(string tenant)
        {
            return this.extensions.AvailableExtensions().ToArray();
        }

        /// <summary>
        /// Gets all features for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant which modules will be get.</param>
        /// <returns>An array of objects representing the modules.</returns>
        public FeatureDescriptor[] GetFeatures(string tenant)
        {
            IEnumerable<FeatureDescriptor> featureDescriptors = this.extensions.AvailableFeatures();
            return featureDescriptors.ToArray();
        }

        /// <summary>
        /// Enables the specified feature.
        /// </summary>
        /// <param name="tenant">The name of the tenant for which the feature will be enabled.</param>
        /// <param name="name">The name of the feature to enable.</param>
        /// <param name="includeDependencies">True to enable dependant features; otherwise, false.</param>
        public void EnableFeature(string tenant, string name, bool includeDependencies)
        {
            this.features.EnableFeatures(new[] { name }, includeDependencies);
        }

        /// <summary>
        /// Disables the specified feature.
        /// </summary>
        /// <param name="tenant">The name of the tenant for which the feature will be disabled.</param>
        /// <param name="name">The name of the feature to disable.</param>
        /// <param name="includeDependencies">True to disable dependant features; otherwise, false.</param>
        public void DisableFeature(string tenant, string name, bool includeDependencies)
        {
            this.features.DisableFeatures(new[] { name }, includeDependencies);
        }

        /// <summary>
        /// Gets all themes for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>An array of objects representing themes from the specified tenant.</returns>
        public OrchardTheme[] GetThemes(string tenant)
        {
            IEnumerable<string> featuresThatNeedUpdate = migrations.GetFeaturesThatNeedUpdate();
            FeatureDescriptor[] tenantFeatures = this.GetFeatures(tenant);

            string currentThemeId = this.services.WorkContext.CurrentSite.As<ThemeSiteSettingsPart>().CurrentThemeName;

            return extensions.AvailableExtensions()
                .Where(d => DefaultExtensionTypes.IsTheme(d.ExtensionType))
                .Where(d => d.Tags != null && d.Tags.Split(',').Any(
                    t => t.Trim().Equals("hidden", StringComparison.OrdinalIgnoreCase)) == false)
                .Select(
                    d => new OrchardTheme
                    {
                        Module = d,
                        Activated = d.Id == currentThemeId,
                        NeedsUpdate = featuresThatNeedUpdate.Contains(d.Id),
                        TenantName = tenant
                    })
                .ToArray();
        }

        /// <summary>
        /// Enables the specified theme for a tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant for which the theme will be enabled.</param>
        /// <param name="id">The identifier of the theme to enable.</param>
        public void EnableTheme(string tenant, string id)
        {
            ExtensionDescriptor theme = this.extensions.AvailableExtensions().FirstOrDefault(x => x.Id == id);
            if (theme == null)
            {
                throw new ArgumentException("Could not find theme '" + id + "'.", "id");
            }

            if (features.GetEnabledFeatures().All(sf => sf.Id != theme.Id))
            {
                themes.EnableThemeFeatures(theme.Id);
            }

            siteThemes.SetSiteTheme(theme.Id);
        }
    }
}