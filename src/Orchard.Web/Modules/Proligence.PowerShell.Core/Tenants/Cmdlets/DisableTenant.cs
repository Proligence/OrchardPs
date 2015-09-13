using System.Management.Automation;
using Orchard.Environment.Configuration;

namespace Proligence.PowerShell.Core.Tenants.Cmdlets {
    [Cmdlet(VerbsLifecycle.Disable, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class DisableTenant : AlterTenantCmdletBase {
        protected override bool AllowAlterDefaultTenant {
            get { return false; }
        }

        protected override string GetActionName() {
            return "Disable";
        }

        protected override bool PerformAction(ShellSettings tenant) {
            if (tenant.State != TenantState.Disabled) {
                tenant.State = TenantState.Disabled;
                return true;
            }

            return false;
        }
    }
}