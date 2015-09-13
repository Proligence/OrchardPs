using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Orchard.ContentManagement;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;
using Orchard.Environment.Features;
using Orchard.Themes.Models;
using Orchard.Themes.Services;
using Proligence.PowerShell.Core.Modules.Items;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Modules.Cmdlets {
    [Cmdlet(VerbsLifecycle.Enable, "OrchardTheme", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EnableOrchardTheme : AlterOrchardFeatureCmdletBase<OrchardTheme> {
        protected override IEnumerable<OrchardTheme> GetTenantFeatures(string tenantName) {
            return this.UsingWorkContextScope(
                tenantName,
                scope => {
                    var migrations = scope.Resolve<IDataMigrationManager>();
                    var extensions = scope.Resolve<IExtensionManager>();
                    IEnumerable<string> featuresThatNeedUpdate = migrations.GetFeaturesThatNeedUpdate();
                    string currentThemeId = scope.WorkContext.CurrentSite.As<ThemeSiteSettingsPart>().CurrentThemeName;

                    return extensions.AvailableExtensions()
                        .Where(d => DefaultExtensionTypes.IsTheme(d.ExtensionType))
                        .Where(d => d.Tags != null && d.Tags.Split(',').Any(
                            t => t.Trim().Equals("hidden", StringComparison.OrdinalIgnoreCase)) == false)
                        .Select(
                            d => new OrchardTheme {
                                Module = d,
                                Activated = d.Id == currentThemeId,
                                NeedsUpdate = featuresThatNeedUpdate.Contains(d.Id),
                                TenantName = tenantName
                            })
                        .ToArray();
                });
        }

        protected override string GetFeatureId(OrchardTheme feature) {
            return feature.Name;
        }

        protected override string GetActionName(OrchardTheme feature, string tenantName) {
            return "Enable Theme";
        }

        protected override void PerformAction(OrchardTheme feature, string tenantName) {
            this.UsingWorkContextScope(
                tenantName,
                scope => {
                    var extensions = scope.Resolve<IExtensionManager>();
                    var features = scope.Resolve<IFeatureManager>();
                    ExtensionDescriptor theme = extensions.AvailableExtensions().FirstOrDefault(x => x.Id == feature.Id);
                    if (theme == null) {
                        throw new ArgumentException("Could not find theme '" + feature.Id + "'.");
                    }

                    var themeService = scope.Resolve<IThemeService>();
                    if (features.GetEnabledFeatures().All(sf => sf.Id != theme.Id)) {
                        themeService.EnableThemeFeatures(theme.Id);
                    }

                    var siteThemeService = scope.Resolve<ISiteThemeService>();
                    siteThemeService.SetSiteTheme(theme.Id);
                });
        }
    }
}