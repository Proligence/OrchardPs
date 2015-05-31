namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement;
    using Proligence.PowerShell.Core.Content.Nodes;

    [Cmdlet(VerbsCommon.Copy, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class CopyContentItem : AlterContentItemCmdletBase
    {
        protected override string GetActionName()
        {
            return "Copy";
        }

        protected override void PerformAction(IContentManager contentManager, ContentItem contentItem)
        {
            var newContentItem = contentManager.Clone(contentItem);
            this.WriteObject(ContentItemNode.BuildPSObject(newContentItem));
        }
    }
}