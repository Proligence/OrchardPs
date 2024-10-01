using System.Management.Automation;
using Orchard.ContentManagement;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("rcit")]
    [Cmdlet(VerbsCommon.Remove, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class RemoveContentItem : AlterContentItemCmdletBase {
        [Alias("d")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentItemObject", Mandatory = false)]
        public SwitchParameter Destroy { get; set; }

        protected override string GetActionName() {
            return Destroy.ToBool()
                ? "Destroy"
                : "Remove";
        }

        protected override void PerformAction(IContentManager contentManager, ContentItem contentItem) {
            if (Destroy.ToBool()) {
                contentManager.Destroy(contentItem);
            }
            else {
                contentManager.Remove(contentItem);
            }
        }
    }
}