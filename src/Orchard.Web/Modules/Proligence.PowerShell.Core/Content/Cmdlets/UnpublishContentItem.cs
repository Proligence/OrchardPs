using System.Management.Automation;
using Orchard.ContentManagement;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [Cmdlet(VerbsData.Unpublish, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class UnpublishContentItem : AlterContentItemCmdletBase {
        protected override string GetActionName() {
            return "Unpublish";
        }

        protected override void PerformAction(IContentManager contentManager, ContentItem contentItem) {
            contentManager.Unpublish(contentItem);
        }
    }
}