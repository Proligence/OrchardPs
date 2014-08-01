namespace Proligence.PowerShell.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Utilities;

    /// <summary>
    /// Implements the <c>Get-OrchardTheme</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "OrchardTheme", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardTheme : OrchardCmdlet, ITenantFilterCmdlet
    {
        /// <summary>
        /// The modules agent instance.
        /// </summary>
        private IModulesAgent modulesAgent;

        /// <summary>
        /// The tenant agent instance.
        /// </summary>
        private ITenantAgent tenantAgent;

        /// <summary>
        /// All available Orchard tenants.
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

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            this.tenants = this.tenantAgent
                .GetTenants()
                .Where(t => t.State == TenantState.Running)
                .ToArray();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            IEnumerable<ShellSettings> filteredTenants = CmdletHelper.FilterTenants(this, this.tenants);

            var themes = new List<OrchardTheme>();

            foreach (ShellSettings tenant in filteredTenants)
            {
                OrchardTheme[] tenantThemes = this.modulesAgent.GetThemes(tenant.Name);
                themes.AddRange(tenantThemes);
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
    }
}