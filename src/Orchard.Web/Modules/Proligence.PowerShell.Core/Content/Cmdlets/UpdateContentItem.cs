using System.Linq;
using System.Management.Automation;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("ucit")]
    [Cmdlet(VerbsData.Update, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class UpdateContentItem : TenantCmdlet {
        [Alias("ci")]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true)]
        public ContentItem ContentItem { get; set; }

        [Alias("vo")]
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public VersionOptionsEnum? VersionOptions { get; set; }

        protected override void ProcessRecord(ShellSettings tenant) {
            this.UsingWorkContextScope(tenant.Name, scope => {
                var contentManager = scope.Resolve<IContentManager>();
                var contentItem = GetContentItem(contentManager, tenant.Name);
                if (contentItem != null) {
                    var target = "Content Item: " + contentItem.Id + ", Tenant: " + tenant.Name;
                    var action = "Update" + (VersionOptions != null
                        ? " " + VersionOptions.Value
                        : string.Empty);
                    if (ShouldProcess(target, action)) {
                        UpdateContentItemData(ContentItem, contentItem);
                    }
                }
            });
        }

        private ContentItem GetContentItem(IContentManager contentManager, string tenantName) {
            ContentItem item = VersionOptions != null
                ? contentManager.Get(ContentItem.Id, VersionOptions.Value.ToVersionOptions())
                : contentManager.Get(ContentItem.Id);

            if (item == null) {
                WriteError(Error.InvalidArgument(
                    "Failed to find content item with ID '" + ContentItem.Id + "' in tenant '" + tenantName + "'.",
                    "FailedToFindContentItem"));
            }

            return item;
        }

        private void UpdateContentItemData(ContentItem source, ContentItem target) {
            foreach (var sourcePart in source.Parts) {
                var targetPart = target.Parts.FirstOrDefault(part => part.GetType() == sourcePart.GetType());
                if (targetPart != null) {
                    foreach (
                        var propertyInfo in sourcePart.GetType().GetProperties().Where(pi => pi.CanRead && pi.CanWrite)) {
                        // Skip content part records, content part definitions properties
                        if (propertyInfo.PropertyType.IsAssignableFrom(typeof (ContentPartRecord)) ||
                            propertyInfo.PropertyType.IsAssignableFrom(typeof (ContentTypePartDefinition))) {
                            continue;
                        }

                        // Skip content item property
                        if (propertyInfo.Name == "ContentItem") {
                            continue;
                        }

                        propertyInfo.SetValue(targetPart, propertyInfo.GetValue(sourcePart));
                    }
                }
            }
        }
    }
}