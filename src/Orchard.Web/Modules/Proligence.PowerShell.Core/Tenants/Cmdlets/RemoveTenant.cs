using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Autofac;
using Orchard.Environment.Configuration;
using Orchard.FileSystems.AppData;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Tenants.Cmdlets {
    [Cmdlet(VerbsCommon.Remove, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTenant : OrchardCmdlet {
        private IShellSettingsManager _shellSettingsManager;
        private IAppDataFolder _appDataFolder;

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings Tenant { get; set; }

        protected override void BeginProcessing() {
            base.BeginProcessing();
            _shellSettingsManager = OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
            _appDataFolder = OrchardDrive.ComponentContext.Resolve<IAppDataFolder>();
        }

        protected override void ProcessRecord() {
            if (ParameterSetName == "Default") {
                InvokeRemoveTenant(Name);
            }
            else if (ParameterSetName == "TenantObject") {
                InvokeRemoveTenant(Tenant.Name);
            }
        }

        private void InvokeRemoveTenant(string tenantName) {
            if (ShouldProcess("Tenant: " + tenantName, "Remove")) {
                try {
                    if (string.IsNullOrEmpty(tenantName)) {
                        WriteError(Error.ArgumentNull("CannotRemoveTenant"));
                    }

                    if (tenantName == ShellSettings.DefaultName) {
                        WriteError(Error.InvalidOperation("Cannot remove default tenant.", "CannotRemoveTenant"));
                        return;
                    }

                    ShellSettings settings = _shellSettingsManager.LoadSettings().FirstOrDefault(
                        x => x.Name == tenantName);

                    if (settings == null) {
                        WriteError(Error.FailedToFindTenant(tenantName));
                        return;
                    }

                    string filePath = Path.Combine(Path.Combine("Sites", settings.Name), "Settings.txt");
                    _appDataFolder.DeleteFile(filePath);
                }
                catch (Exception ex) {
                    WriteError(Error.Generic(ex, "FailedToRemoveTenant"));
                }
            }
        }
    }
}