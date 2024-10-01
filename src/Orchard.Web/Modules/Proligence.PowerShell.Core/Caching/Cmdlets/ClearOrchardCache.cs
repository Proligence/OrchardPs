using System.Management.Automation;
using Orchard.Caching.Services;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Caching.Cmdlets {
    [OrchardFeature("Proligence.PowerShell.Caching")]
    [Cmdlet(VerbsCommon.Clear, "OrchardCache", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class ClearOrchardCache : TenantCmdlet {
        protected override void ProcessRecord(ShellSettings tenant) {
            this.UsingWorkContextScope(tenant.Name, scope => {
                if (ShouldProcess("OrchardCache", "Clear")) {
                    var cacheManager = scope.Resolve<ICacheService>();
                    cacheManager.Clear();
                }
            });
        }
    }
}