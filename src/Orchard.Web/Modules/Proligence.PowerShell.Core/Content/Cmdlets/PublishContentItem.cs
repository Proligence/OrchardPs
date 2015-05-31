namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement;

    [Cmdlet(VerbsData.Publish, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class PublishContentItem : AlterContentItemCmdletBase
    {
        protected override string GetActionName()
        {
            return "Publish";
        }

        protected override void PerformAction(IContentManager contentManager, ContentItem contentItem)
        {
            contentManager.Publish(contentItem);
        }
    }
}