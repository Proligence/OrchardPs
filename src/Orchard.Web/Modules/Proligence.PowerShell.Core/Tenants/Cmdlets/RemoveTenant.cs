namespace Proligence.PowerShell.Core.Tenants.Cmdlets
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Orchard.FileSystems.AppData;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsCommon.Remove, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveTenant : OrchardCmdlet
    {
        private IShellSettingsManager shellSettingsManager;
        private IAppDataFolder appDataFolder;

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings Tenant { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.shellSettingsManager = this.OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
            this.appDataFolder = this.OrchardDrive.ComponentContext.Resolve<IAppDataFolder>();
        }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "Default")
            {
                this.InvokeRemoveTenant(this.Name);
            }
            else if (this.ParameterSetName == "TenantObject")
            {
                this.InvokeRemoveTenant(this.Tenant.Name);
            }
        }

        private void InvokeRemoveTenant(string tenantName)
        {
            if (this.ShouldProcess("Tenant: " + tenantName, "Remove"))
            {
                try
                {
                    if (string.IsNullOrEmpty(tenantName))
                    {
                        this.WriteError(Error.ArgumentNull("CannotRemoveTenant"));
                    }

                    if (tenantName == ShellSettings.DefaultName)
                    {
                        this.WriteError(Error.InvalidOperation("Cannot remove default tenant.", "CannotRemoveTenant"));
                        return;
                    }

                    ShellSettings settings = this.shellSettingsManager.LoadSettings().FirstOrDefault(
                        x => x.Name == tenantName);

                    if (settings == null)
                    {
                        this.WriteError(Error.FailedToFindTenant(tenantName));
                        return;
                    }

                    string filePath = Path.Combine(Path.Combine("Sites", settings.Name), "Settings.txt");
                    this.appDataFolder.DeleteFile(filePath);
                }
                catch (Exception ex)
                {
                    this.WriteError(Error.Generic(ex, "FailedToRemoveTenant"));
                }
            }
        }
    }
}