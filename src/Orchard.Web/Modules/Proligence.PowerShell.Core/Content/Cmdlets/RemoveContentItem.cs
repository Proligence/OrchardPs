namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("rcit")]
    [Cmdlet(VerbsCommon.Remove, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class RemoveContentItem : AlterContentItemCmdletBase
    {
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentItemObject", Mandatory = false)]
        public SwitchParameter Destroy { get; set; }

        protected override string GetActionName()
        {
            return this.Destroy.ToBool() ? "Destroy" : "Remove";
        }

        protected override void PerformAction(IContentManager contentManager, ContentItem contentItem)
        {
            if (this.Destroy.ToBool())
            {
                contentManager.Destroy(contentItem);
            }
            else
            {
                contentManager.Remove(contentItem);
            }
        }
    }
}