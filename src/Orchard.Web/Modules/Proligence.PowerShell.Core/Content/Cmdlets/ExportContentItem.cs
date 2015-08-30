namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using System.Xml.Linq;
    using Orchard.ContentManagement;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("ecit")]
    [Cmdlet(VerbsData.Export, "ContentItem", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class ExportContentItem : AlterContentItemCmdletBase
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
            return "Export";
        }

        protected override void PerformAction(IContentManager contentManager, ContentItem contentItem)
        {
            string xml = contentManager.Export(contentItem).ToString(SaveOptions.None);
            this.WriteObject(xml);
        }
    }
}