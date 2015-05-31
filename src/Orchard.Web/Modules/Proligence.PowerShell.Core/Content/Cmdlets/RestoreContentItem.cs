namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement;
    using Proligence.PowerShell.Core.Content.Nodes;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsData.Restore, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class RestoreContentItem : AlterContentItemCmdletBase
    {
        /// <remarks>
        /// As this parameter doesn't have any parameter sets, it won't be available for the Restore-ContentItem
        /// cmdlet. It doesn't make any since, since we need to specify a version number to restore.
        /// </remarks>>
        public override VersionOptionsEnum? VersionOptions { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "ContentItemObject", Mandatory = false, Position = 2)]
        public override int? Version { get; set; }

        protected override string GetActionName()
        {
            return "Restore";
        }

        protected override void PerformAction(IContentManager contentManager, ContentItem contentItem)
        {
            if (this.Version != null)
            {
                var versionOptions = Orchard.ContentManagement.VersionOptions.Number(this.Version.Value);
                var restoredItem = contentManager.Restore(contentItem, versionOptions);
                this.WriteObject(ContentItemNode.BuildPSObject(restoredItem));
            }
            else
            {
                this.WriteError(Error.InvalidArgument(
                    "The VersionOptions or Version parameter must be specified.",
                    "ContentItemVersionNotSpecified"));
            }
        }
    }
}