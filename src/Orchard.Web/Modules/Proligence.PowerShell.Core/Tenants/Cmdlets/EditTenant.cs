namespace Proligence.PowerShell.Core.Tenants.Cmdlets
{
    using System.Collections.Generic;
    using System.Management.Automation;
    using Orchard.Environment.Configuration;

    [Cmdlet(VerbsData.Edit, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EditTenant : AlterTenantCmdletBase
    {
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string RequestUrlHost { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string RequestUrlPrefix { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string DataConnectionString { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string DataProvider { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string DataTablePrefix { get; set; }

        protected override bool AllowAlterDefaultTenant
        {
            get { return true; }
        }

        protected override string GetActionName()
        {
            var list = new List<string>();

            if (this.RequestUrlHost != null)
            {
                list.Add("RequestUrlHost = '" + this.RequestUrlHost + "'");
            }

            if (this.RequestUrlPrefix != null)
            {
                list.Add("RequestUrlPrefix = '" + this.RequestUrlPrefix + "'");
            }

            if (this.DataConnectionString != null)
            {
                list.Add("DataConnectionString = '" + this.DataConnectionString + "'");
            }

            if (this.DataProvider != null)
            {
                list.Add("DataProvider = '" + this.DataProvider + "'");
            }

            if (this.DataTablePrefix != null)
            {
                list.Add("DataTablePrefix = '" + this.DataTablePrefix + "'");
            }

            return "Set " + string.Join(", ", list);
        }

        protected override bool PerformAction(ShellSettings tenant)
        {
            if (this.RequestUrlHost != null)
            {
                tenant.RequestUrlHost = this.RequestUrlHost;
            }

            if (this.RequestUrlPrefix != null)
            {
                tenant.RequestUrlPrefix = this.RequestUrlPrefix;
            }

            if (this.DataConnectionString != null)
            {
                tenant.DataConnectionString = this.DataConnectionString;
            }

            if (this.DataProvider != null)
            {
                tenant.DataProvider = this.DataProvider;
            }

            if (this.DataTablePrefix != null)
            {
                tenant.DataTablePrefix = this.DataTablePrefix;
            }

            return true;
        }
    }
}