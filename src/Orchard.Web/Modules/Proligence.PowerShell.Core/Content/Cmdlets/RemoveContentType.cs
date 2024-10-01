using System.Management.Automation;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [Cmdlet(VerbsCommon.Remove, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveContentType : AlterContentTypeCmdletBase {
        public RemoveContentType()
            : base(failIfDoesNotExist: true) {
        }

        [Alias("n")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 1)]
        public override string Name {
            get {
                return ContentType != null
                    ? ContentType.Name
                    : base.Name;
            }
            set { base.Name = value; }
        }

        [Alias("ct")]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentType { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public override string Tenant { get; set; }

        protected override string GetActionName() {
            return "Remove";
        }

        protected override void PerformAction(IContentDefinitionManager contentDefinitionManager) {
            contentDefinitionManager.DeleteTypeDefinition(Name);
        }
    }
}