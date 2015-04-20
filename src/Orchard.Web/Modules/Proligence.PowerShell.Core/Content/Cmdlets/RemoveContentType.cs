namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;

    [Cmdlet(VerbsCommon.Remove, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveContentType : AlterContentTypeCmdletBase
    {
        public RemoveContentType()
            : base(failIfDoesNotExist: true)
        {
        }

        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentType { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public override string Tenant { get; set; }

        protected override string GetActionName()
        {
            return "Remove";
        }

        protected override void PerformAction(IContentDefinitionManager contentDefinitionManager)
        {
            string contentTypeName = this.ContentType != null ? this.ContentType.Name : this.Name;
            contentDefinitionManager.DeleteTypeDefinition(contentTypeName);
        }
    }
}