namespace Proligence.PowerShell.Core.Tenants.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Text.RegularExpressions;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsCommon.New, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class NewTenant : OrchardCmdlet
    {
        private IShellSettingsManager shellSettingsManager;

        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 2)]
        public string RequestUrlHost { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string RequestUrlPrefix { get; set; }

        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string DataConnectionString { get; set; }

        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string DataProvider { get; set; }

        [Mappable]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string DataTablePrefix { get; set; }

        [Parameter(ParameterSetName = "TenantObject", Mandatory = true)]
        public ShellSettings Tenant { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.shellSettingsManager = this.OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
        }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "Default")
            {
                var tenant = new ShellSettings();
                PropertyMapper.Instance.MapProperties(this, tenant);

                this.InvokeCreateTenant(tenant);
            }
            else if (this.ParameterSetName == "TenantObject")
            {
                this.InvokeCreateTenant(this.Tenant);
            }
        }

        private void InvokeCreateTenant(ShellSettings tenant)
        {
            if (this.ShouldProcess("Tenant: " + tenant.Name, "Create"))
            {
                try
                {
                    if (!string.IsNullOrEmpty(tenant.Name) && !Regex.IsMatch(tenant.Name, @"^\w+$"))
                    {
                        this.WriteError(Error.InvalidArgument(
                            "Invalid tenant name. Must contain characters only and no spaces.",
                            "CannotCreateTenant"));
                        
                        return;
                    }

                    if (tenant.Name == ShellSettings.DefaultName)
                    {
                        this.WriteError(Error.InvalidArgument("Invalid tenant name.", "CannotCreateTenant"));
                        return;
                    }

                    ShellSettings defaultTenant = this.shellSettingsManager.LoadSettings().FirstOrDefault(
                        x => x.Name == ShellSettings.DefaultName);

                    if (defaultTenant == null)
                    {
                        this.WriteError(Error.FailedToFindTenant("Default"));
                        return;
                    }

                    tenant.State = TenantState.Uninitialized;
                    tenant.Themes = defaultTenant.Themes;
                    tenant.Modules = defaultTenant.Modules;

                    this.shellSettingsManager.SaveSettings(tenant);
                }
                catch (Exception ex)
                {
                    this.WriteError(Error.NotSpecified(ex, "FailedToCreateTenant"));
                }
            }
        }
    }
}