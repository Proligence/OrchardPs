namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement;
    using Orchard.Data.Migration;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Themes.Models;
    using Proligence.PowerShell.Core.Modules.Items;
    using Proligence.PowerShell.Provider;

    [Cmdlet(VerbsCommon.Get, "OrchardTheme", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardTheme : RetrieveOrchardFeatureCmdletBase<OrchardTheme>
    {
        protected override IEnumerable<OrchardTheme> GetFeatures(string tenant)
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

        protected override string GetFeatureId(OrchardTheme feature)
        {
            return feature.Id;
        }

        protected override string GetFeatureName(OrchardTheme feature)
        {
            return feature.Name;
        }

        protected override bool IsFeatureEnabled(OrchardTheme feature, string tenant)
        {
            return feature.Activated;
        }
    }
}