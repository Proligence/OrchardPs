using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Autofac;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Tenants.Cmdlets {
    [Cmdlet(VerbsCommon.Get, "Tenant", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetTenant : OrchardCmdlet {
        [Alias("n")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 1)]
        public string Name { get; set; }

        [Alias("e")]
        [Parameter(ParameterSetName = "Enabled", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        [Alias("d")]
        [Parameter(ParameterSetName = "Disabled", Mandatory = false)]
        public SwitchParameter Disabled { get; set; }

        protected override void ProcessRecord() {
            foreach (ShellSettings tenant in GetTenantsFromParameters()) {
                WriteObject(tenant);
            }
        }

        private IEnumerable<ShellSettings> GetTenantsFromParameters() {
            var shellSettingsManager = OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
            IEnumerable<ShellSettings> tenants = shellSettingsManager.LoadSettings().ToArray();

            if (!string.IsNullOrEmpty(Name)) {
                tenants = tenants.Where(t => t.Name.WildcardEquals(Name));
            }
            else {
                if (Enabled.ToBool()) {
                    tenants = tenants.Where(t => t.State == TenantState.Running);
                }

                if (Disabled.ToBool()) {
                    tenants = tenants.Where(t => t.State == TenantState.Disabled);
                }
            }

            return tenants;
        }
    }
}