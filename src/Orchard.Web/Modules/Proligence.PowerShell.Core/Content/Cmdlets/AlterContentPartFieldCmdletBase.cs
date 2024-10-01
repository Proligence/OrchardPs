using System.Management.Automation;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    public abstract class AlterContentPartFieldCmdletBase : TenantCmdlet {
        [Alias("cp")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public string ContentPart { get; set; }

        [Alias("cfd")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, Position = 2)]
        public string ContentField { get; set; }

        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentPartDefinition ContentPartObject { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = false)]
        public override string Tenant { get; set; }

        protected abstract string GetActionName(string contentFieldName);
        protected abstract void PerformAction(ContentPartDefinitionBuilder builder, string contentFieldName);

        protected override void ProcessRecord(ShellSettings tenant) {
            ContentPartDefinition contentPart = GetContentPartDefinition(tenant.Name);
            if (contentPart != null) {
                string target = "Content Part: " + contentPart.Name + ", Tenant: " + tenant.Name;

                if (ShouldProcess(target, GetActionName(ContentField))) {
                    this.UsingWorkContextScope(
                        tenant.Name,
                        scope => scope.Resolve<IContentDefinitionManager>()
                            .AlterPartDefinition(
                                contentPart.Name,
                                builder => PerformAction(builder, ContentField)));
                }
            }
        }

        private ContentPartDefinition GetContentPartDefinition(string tenantName) {
            if (ContentPartObject != null) {
                return ContentPartObject;
            }

            if (ContentPart != null) {
                ContentPartDefinition contentPart = this.UsingWorkContextScope(
                    tenantName,
                    scope => scope.Resolve<IContentDefinitionManager>().GetPartDefinition(ContentPart));

                if (contentPart == null) {
                    NotifyFailedToFindContentPart(ContentPart, tenantName);
                }

                return contentPart;
            }

            NotifyFailedToFindContentPart(string.Empty, tenantName);
            return null;
        }

        private void NotifyFailedToFindContentPart(string name, string tenantName) {
            WriteError(Error.InvalidArgument(
                "Failed to find content part '" + name + "' in tenant '" + tenantName + "'.",
                "FailedToFindContentPart"));
        }
    }
}