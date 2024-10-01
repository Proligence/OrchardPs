using System.Collections.Generic;
using System.Management.Automation;
using Orchard.Environment.Configuration;

namespace Proligence.PowerShell.Core.Tenants.Cmdlets {
    [Cmdlet(VerbsData.Edit, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EditTenant : AlterTenantCmdletBase {
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

        protected override bool AllowAlterDefaultTenant {
            get { return true; }
        }

        protected override string GetActionName() {
            var list = new List<string>();

            if (RequestUrlHost != null) {
                list.Add("RequestUrlHost = '" + RequestUrlHost + "'");
            }

            if (RequestUrlPrefix != null) {
                list.Add("RequestUrlPrefix = '" + RequestUrlPrefix + "'");
            }

            if (DataConnectionString != null) {
                list.Add("DataConnectionString = '" + DataConnectionString + "'");
            }

            if (DataProvider != null) {
                list.Add("DataProvider = '" + DataProvider + "'");
            }

            if (DataTablePrefix != null) {
                list.Add("DataTablePrefix = '" + DataTablePrefix + "'");
            }

            return "Set " + string.Join(", ", list);
        }

        protected override bool PerformAction(ShellSettings tenant) {
            if (RequestUrlHost != null) {
                tenant.RequestUrlHost = RequestUrlHost;
            }

            if (RequestUrlPrefix != null) {
                tenant.RequestUrlPrefix = RequestUrlPrefix;
            }

            if (DataConnectionString != null) {
                tenant.DataConnectionString = DataConnectionString;
            }

            if (DataProvider != null) {
                tenant.DataProvider = DataProvider;
            }

            if (DataTablePrefix != null) {
                tenant.DataTablePrefix = DataTablePrefix;
            }

            return true;
        }
    }
}