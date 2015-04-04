namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.Descriptor.Models;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Proligence.PowerShell.Core.Utilities;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsCommon.Get, "OrchardFeature", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardFeature : OrchardCmdlet, ITenantFilterCmdlet
    {
        /// <summary>
        /// Caches all available Orchard tenants.
        /// </summary>
        private ShellSettings[] tenants;

        /// <summary>
        /// Gets or sets the name of the Orchard feature to get.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which the feature will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = false)]        
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which features will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether features should be retrieved from all tenants.
        /// </summary>
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public SwitchParameter FromAllTenants { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only enabled features should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only disabled features should be retrieved.
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

            var features = new List<FeatureDescriptor>();

            foreach (ShellSettings tenant in filteredTenants)
            {
                this.UsingWorkContextScope(
                    tenant.Name,
                    scope =>
                        {
                            var extensions = scope.Resolve<IExtensionManager>();
                            var shellDescriptor = scope.Resolve<ShellDescriptor>();

                            if (this.Enabled.ToBool())
                            {
                                // ReSharper disable once AccessToModifiedClosure
                                features.AddRange(extensions.EnabledFeatures(shellDescriptor));
                            }
                            else if (this.Disabled.ToBool())
                            {
                                // ReSharper disable once AccessToModifiedClosure
                                features.AddRange(
                                    extensions.AvailableFeatures()
                                    .Except(extensions.EnabledFeatures(shellDescriptor)));
                            }
                            else
                            {
                                // ReSharper disable once AccessToModifiedClosure
                                features.AddRange(extensions.AvailableFeatures());
                            }
                        });
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                features = features.Where(f => f.Name.WildcardEquals(this.Name)).ToList();
            }

            foreach (FeatureDescriptor feature in features)
            {
                this.WriteObject(feature);
            }
        }
    }
}