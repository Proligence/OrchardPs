namespace Proligence.PowerShell.Modules.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    using Orchard.Environment.Configuration;

    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements the <c>Enable-OrchardFeature</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Enable, "OrchardTheme", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EnableOrchardTheme : OrchardCmdlet
    {
        /// <summary>
        /// The modules agent proxy instance.
        /// </summary>
        private IModulesAgent modulesAgent;

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
                    this.modulesAgent.EnableTheme(tenantName, theme.Id);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="OrchardTheme"/> object for the specified theme.
        /// </summary>
        /// <param name="tenantName">The name to the tenant for which to get theme.</param>
        /// <param name="themeName">The name of the theme to get.</param>
        /// <returns>The <see cref="OrchardTheme"/> object for the specified theme.</returns>
        private OrchardTheme GetOrchardTheme(string tenantName, string themeName)
        {
            OrchardTheme[] tenantThemes;

            if (!this.themes.TryGetValue(tenantName, out tenantThemes))
            {
                tenantThemes = this.modulesAgent.GetThemes(tenantName).ToArray();
                this.themes.Add(tenantName, tenantThemes);
            }

            return tenantThemes.FirstOrDefault(
                f => f.Name.Equals(themeName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}