using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// The base class for cmdlets which work in the context of an Orchard tenant.
    /// </summary>
    /// <remarks>
    /// This base class adds support for the <c>TenantObject</c>, <c>TenantObject</c> and <c>AllTenant</c>
    /// cmdlet parameters.
    /// </remarks>
    public abstract class TenantCmdlet : OrchardCmdlet {
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public virtual string Tenant { get; set; }

        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public virtual ShellSettings TenantObject { get; set; }

        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public virtual SwitchParameter AllTenants { get; set; }

        protected ShellSettings[] AvailableTenants { get; private set; }

        protected override void BeginProcessing() {
            base.BeginProcessing();

            AvailableTenants = this.Resolve<IShellSettingsManager>()
                .LoadSettings()
                .Where(t => t.State == TenantState.Running)
                .ToArray();
        }

        protected abstract void ProcessRecord(ShellSettings tenant);

        protected override void ProcessRecord() {
            foreach (ShellSettings tenant in FilterTenants(AvailableTenants)) {
                ProcessRecord(tenant);
            }
        }

        private IEnumerable<ShellSettings> FilterTenants(IEnumerable<ShellSettings> tenants) {
            var result = new List<ShellSettings>();

            if (AllTenants.ToBool()) {
                result.AddRange(tenants);
            }
            else if (!string.IsNullOrEmpty(Tenant)) {
                ShellSettings[] namedTenants = tenants
                    .Where(s => s.Name.Equals(Tenant, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (namedTenants.Any()) {
                    result.AddRange(namedTenants);
                }
                else {
                    WriteError(Error.FailedToFindTenant(Tenant));
                }
            }
            else if (TenantObject != null) {
                result.Add(TenantObject);
            }
            else {
                ShellSettings currentTenant = this.GetCurrentTenant();
                if (currentTenant != null) {
                    result.Add(currentTenant);
                }
                else {
                    ShellSettings defaultTenant = tenants.SingleOrDefault(
                        s => s.Name.Equals("Default", StringComparison.OrdinalIgnoreCase));

                    if (defaultTenant != null) {
                        result.Add(defaultTenant);
                    }
                    else {
                        WriteError(Error.FailedToFindTenant("Default"));
                    }
                }
            }

            return result;
        }
    }
}