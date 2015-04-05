namespace Proligence.PowerShell.Core.Modules.Nodes 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.ContentManagement;
    using Orchard.Data.Migration;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Themes.Models;
    using Proligence.PowerShell.Core.Modules.Items;
    using Proligence.PowerShell.Core.Utilities;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Items;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    
    /// <summary>
    /// Implements a VFS node which groups <see cref="ThemeNode"/> nodes for a single Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardTheme")]
    public class ThemesNode : ContainerNode
    {
        private readonly IExtensionManager extensions;
        private readonly IDataMigrationManager migrations;

        public ThemesNode(IPowerShellVfs vfs, IExtensionManager extensions, IDataMigrationManager migrations)
            : base(vfs, "Themes")
        {
            this.migrations = migrations;
            this.extensions = extensions;
            this.Item = new CollectionItem(this) 
            {
                Name = "Themes",
                Description = "Contains all themes available in the current tenant."
            };
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() 
        {
            string tenantName = this.GetCurrentTenantName();
            if (tenantName == null) 
            {
                return new VfsNode[0];
            }

            IEnumerable<OrchardTheme> themes = this.GetThemes(tenantName);
            return themes.Select(theme => new ThemeNode(this.Vfs, theme));
        }

        private IEnumerable<OrchardTheme> GetThemes(string tenant)
        {
            IEnumerable<string> featuresThatNeedUpdate = this.migrations.GetFeaturesThatNeedUpdate();

            string currentThemeId = this.UsingWorkContextScope(
                tenant,
                scope => scope.WorkContext.CurrentSite.As<ThemeSiteSettingsPart>().CurrentThemeName);

            return this.extensions.AvailableExtensions()
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
    }
}