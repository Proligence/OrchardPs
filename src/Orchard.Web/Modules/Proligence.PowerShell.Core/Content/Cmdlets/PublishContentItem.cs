namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement;

    [Cmdlet(VerbsData.Publish, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class PublishContentItem : AlterContentItemCmdletBase
    {
        [Alias("vo")]
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentItemObject", Mandatory = false)]
        public override VersionOptionsEnum? VersionOptions { get; set; }

        [Alias("v")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentItemObject", Mandatory = false)]
        public override int? Version { get; set; }

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