using System;
using System.Linq;
using System.Management.Automation;
using Autofac;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Tenants.Cmdlets {
    public abstract class AlterTenantCmdletBase : OrchardCmdlet {
        private IShellSettingsManager _shellSettingsManager;

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "TenantObject", ValueFromPipeline = true)]
        public ShellSettings Tenant { get; set; }

        protected abstract bool AllowAlterDefaultTenant { get; }

        protected override void BeginProcessing() {
            base.BeginProcessing();
            _shellSettingsManager = OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
        }

        protected override void ProcessRecord() {
            if (ParameterSetName == "Default") {
                AlterTenant(Name);
            }
            else if (ParameterSetName == "TenantObject") {
                AlterTenant(Tenant.Name);
            }
        }

        protected abstract string GetActionName();
        protected abstract bool PerformAction(ShellSettings tenant);

        private void AlterTenant(string name) {
            if (ShouldProcess("Tenant: " + name, GetActionName())) {
                try {
                    ShellSettings tenant = _shellSettingsManager.LoadSettings()
                        .FirstOrDefault(x => x.Name == name);

                    if (tenant == null) {
                        WriteError(Error.FailedToFindTenant(name));
                        return;
                    }

                    if (!AllowAlterDefaultTenant) {
                        if (tenant.Name == ShellSettings.DefaultName) {
                            WriteError(Error.CannotAlterDefaultTenant());
                            return;
                        }
                    }

                    if (PerformAction(tenant)) {
                        _shellSettingsManager.SaveSettings(tenant);
                    }
                }
                catch (Exception ex) {
                    WriteError(Error.NotSpecified(ex, "AlterTenantFailed", name));
                }
            }
        }
    }
}