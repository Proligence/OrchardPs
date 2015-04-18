namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider.Utilities;

    /// <summary>
    /// The base class for cmdlets which work in the context of an Orchard tenant.
    /// </summary>
    /// <remarks>
    /// This base class adds support for the <c>TenantObject</c>, <c>TenantObject</c> and <c>AllTenant</c>
    /// cmdlet parameters.
    /// </remarks>
    public abstract class TenantCmdlet : OrchardCmdlet
    {
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public virtual string Tenant { get; set; }

        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public virtual ShellSettings TenantObject { get; set; }

        [Parameter(ParameterSetName = "AllTenants", Mandatory = true)]
        public virtual SwitchParameter AllTenants { get; set; }

        protected ShellSettings[] AvailableTenants { get; private set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            this.AvailableTenants = this.Resolve<IShellSettingsManager>()
                .LoadSettings()
                .Where(t => t.State == TenantState.Running)
                .ToArray();
        }

        protected abstract void ProcessRecord(ShellSettings tenant);

        protected override void ProcessRecord()
        {
            foreach (ShellSettings tenant in this.FilterTenants(this.AvailableTenants))
            {
                this.ProcessRecord(tenant);
            }
        }

        private IEnumerable<ShellSettings> FilterTenants(IEnumerable<ShellSettings> tenants)
        {
            var result = new List<ShellSettings>();

            if (this.AllTenants.ToBool())
            {
                result.AddRange(tenants);
            }
            else if (!string.IsNullOrEmpty(this.Tenant))
            {
                ShellSettings[] namedTenants = tenants
                    .Where(s => s.Name.Equals(this.Tenant, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (namedTenants.Any())
                {
                    result.AddRange(namedTenants);
                }
                else
                {
                    this.WriteError(Error.FailedToFindTenant(this.Tenant));
                }
            }
            else if (this.TenantObject != null)
            {
                result.Add(this.TenantObject);
            }
            else
            {
                ShellSettings currentTenant = this.GetCurrentTenant();
                if (currentTenant != null)
                {
                    result.Add(currentTenant);
                }
                else
                {
                    ShellSettings defaultTenant = tenants.SingleOrDefault(
                        s => s.Name.Equals("Default", StringComparison.OrdinalIgnoreCase));

                    if (defaultTenant != null)
                    {
                        result.Add(defaultTenant);
                    }
                    else
                    {
                        this.WriteError(Error.FailedToFindTenant("Default"));
                    }
                }
            }

            return result;
        }
    }
}