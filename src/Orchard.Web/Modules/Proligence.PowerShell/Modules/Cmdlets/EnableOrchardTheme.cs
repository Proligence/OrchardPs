namespace Proligence.PowerShell.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement;
    using Orchard.Data.Migration;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Environment.Features;
    using Orchard.Themes.Models;
    using Orchard.Themes.Services;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Utilities;

    /// <summary>
    /// Implements the <c>Enable-OrchardFeature</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Enable, "OrchardTheme", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EnableOrchardTheme : OrchardCmdlet
    {
        /// <summary>
        /// Cached list of all Orchard themes for each tenant.
        /// </summary>
        private IDictionary<string, OrchardTheme[]> themes;

        /// <summary>
        /// Gets or sets the name of the theme to enable.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which the theme will be enabled.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OrchardTheme"/> object which represents the orchard theme to enable.
        /// </summary>
        [Parameter(ParameterSetName = "ThemeObject", ValueFromPipeline = true)]
        public OrchardTheme Theme { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.themes = new Dictionary<string, OrchardTheme[]>();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            string tenantName = null;

            if ((this.ParameterSetName != "ThemeObject") && string.IsNullOrEmpty(this.Tenant))
            {
                // Get feature for current tenant if tenant name not specified
                ShellSettings tenant = this.GetCurrentTenant();
                tenantName = tenant != null ? tenant.Name : "Default";
            }

            if (!string.IsNullOrEmpty(this.Tenant))
            {
                tenantName = this.Tenant;
            }

            OrchardTheme theme = null;

            if (this.ParameterSetName == "Default")
            {
                theme = this.GetOrchardTheme(tenantName, this.Name);
                if (theme == null)
                {
                    this.WriteError(
                        new ArgumentException("Failed to find theme with name '" + this.Name + "'."),
                        "FailedToFindTheme",
                        ErrorCategory.InvalidArgument);
                }
            }
            else if (this.ParameterSetName == "ThemeObject")
            {
                theme = this.Theme;
                tenantName = this.Theme.TenantName;
            }

            if (theme != null)
            {
                if (this.ShouldProcess("Theme: " + theme.Name + ", Tenant: " + tenantName, "Enable Theme"))
                {
                    this.EnableTheme(tenantName, theme.Id);
                }
            }
        }

        private IEnumerable<OrchardTheme> GetThemes(string tenant)
        {
            return this.UsingWorkContextScope(
                tenant,
                scope =>
                    {
                        var migrations = scope.Resolve<IDataMigrationManager>();
                        var extensions = scope.Resolve<IExtensionManager>();
                        IEnumerable<string> featuresThatNeedUpdate = migrations.GetFeaturesThatNeedUpdate();
                        string currentThemeId = scope.WorkContext.CurrentSite.As<ThemeSiteSettingsPart>().CurrentThemeName;

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
                    });
        }

        private OrchardTheme GetOrchardTheme(string tenantName, string themeName)
        {
            OrchardTheme[] tenantThemes;

            if (!this.themes.TryGetValue(tenantName, out tenantThemes))
            {
                tenantThemes = this.GetThemes(tenantName).ToArray();
                this.themes.Add(tenantName, tenantThemes);
            }

            return tenantThemes.FirstOrDefault(
                f => f.Name.Equals(themeName, StringComparison.CurrentCultureIgnoreCase));
        }

        private void EnableTheme(string tenant, string id)
        {
            this.UsingWorkContextScope(
                tenant,
                scope =>
                    {
                        var extensions = scope.Resolve<IExtensionManager>();
                        var features = scope.Resolve<IFeatureManager>();
                        ExtensionDescriptor theme = extensions.AvailableExtensions().FirstOrDefault(x => x.Id == id);
                        if (theme == null)
                        {
                            throw new ArgumentException("Could not find theme '" + id + "'.", "id");
                        }

                        var themeService = scope.Resolve<IThemeService>();
                        if (features.GetEnabledFeatures().All(sf => sf.Id != theme.Id))
                        {
                            themeService.EnableThemeFeatures(theme.Id);
                        }

                        var siteThemeService = scope.Resolve<ISiteThemeService>();
                        siteThemeService.SetSiteTheme(theme.Id);
                    });
        }
    }
}