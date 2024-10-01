using System.Management.Automation;
using Orchard.Environment.Configuration;

namespace Proligence.PowerShell.Core.Tenants.Cmdlets {
    [Cmdlet(VerbsLifecycle.Disable, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EnableTenant : AlterTenantCmdletBase {
        protected override bool AllowAlterDefaultTenant {
            get { return false; }
        }

        protected override string GetActionName() {
            return "Enable";
        }

        protected override bool PerformAction(ShellSettings tenant) {
            if (tenant.State != TenantState.Running) {
                tenant.State = TenantState.Running;
                return true;
            }

            return false;
        }
    }
}