namespace Proligence.PowerShell.Core.Modules.Cmdlets
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
    using Orchard.Themes.Models;
    using Proligence.PowerShell.Core.Modules.Items;
    using Proligence.PowerShell.Core.Utilities;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsCommon.Get, "OrchardTheme", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardTheme : OrchardCmdlet, ITenantFilterCmdlet
    {
        /// <summary>
        /// Caches all available Orchard tenants.
        /// </summary>
        private ShellSettings[] tenants;

        /// <summary>
        /// Gets or sets the name of the Orchard theme to get.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which themes will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which themes will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether themes should be retrieved from all tenants.
        /// </summary>
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public SwitchParameter FromAllTenants { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only enabled themes should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only disabled themes should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Disabled { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            this.tenants = this.Resolve<IShellSettingsManager>()
                .LoadSettings()
                .Where(t => t.State == TenantState.Running)
                .ToArray();
        }

        protected override void ProcessRecord()
        {
            IEnumerable<ShellSettings> filteredTenants = CmdletHelper.FilterTenants(this, this.tenants);

            var themes = new List<OrchardTheme>();

            foreach (ShellSettings tenant in filteredTenants)
            {
                themes.AddRange(this.GetThemes(tenant.Name));
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                themes = themes.Where(f => f.Name.WildcardEquals(this.Name)).ToList();
            }

            if (this.Enabled.ToBool())
            {
                themes = themes.Where(f => f.Enabled).ToList();
            }

            if (this.Disabled.ToBool())
            {
                themes = themes.Where(f => !f.Enabled).ToList();
            }

            foreach (OrchardTheme theme in themes)
            {
                this.WriteObject(theme);
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
    }
}