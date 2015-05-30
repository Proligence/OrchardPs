namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.ContentManagement.Records;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [CmdletAlias("ucit")]
    [Cmdlet(VerbsData.Update, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class UpdateContentItem : TenantCmdlet
    {
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1, ValueFromPipeline = true)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true)]
        public ContentItem ContentItem { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public VersionOptionsEnum? VersionOptions { get; set; }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            this.UsingWorkContextScope(tenant.Name, scope =>
            {
                var contentManager = scope.Resolve<IContentManager>();
                var contentItem = this.GetContentItem(contentManager, tenant.Name);
                if (contentItem != null)
                {
                    var target = "Content Item: " + contentItem.Id + ", Tenant: " + tenant.Name;
                    var action = "Update" + (this.VersionOptions != null ? " " + this.VersionOptions.Value : string.Empty);
                    if (this.ShouldProcess(target, action))
                    {
                        this.UpdateContentItemData(this.ContentItem, contentItem);
                    }
                }
            });
        }

        private ContentItem GetContentItem(IContentManager contentManager, string tenantName)
        {
            ContentItem item = this.VersionOptions != null
                ? contentManager.Get(this.ContentItem.Id, this.VersionOptions.Value.ToVersionOptions())
                : contentManager.Get(this.ContentItem.Id);

            if (item == null)
            {
                this.WriteError(Error.InvalidArgument(
                    "Failed to find content item with ID '" + this.ContentItem.Id + "' in tenant '" + tenantName + "'.",
                    "FailedToFindContentItem"));
            }

            return item;
        }

        private void UpdateContentItemData(ContentItem source, ContentItem target)
        {
            foreach (var sourcePart in source.Parts)
            {
                var targetPart = target.Parts.FirstOrDefault(part => part.GetType() == sourcePart.GetType());
                if (targetPart != null)
                {
                    foreach (var propertyInfo in sourcePart.GetType().GetProperties().Where(pi => pi.CanRead && pi.CanWrite))
                    {
                        // Skip content part records, content part definitions properties
                        if (propertyInfo.PropertyType.IsAssignableFrom(typeof(ContentPartRecord)) ||
                            propertyInfo.PropertyType.IsAssignableFrom(typeof(ContentTypePartDefinition)))
                        {
                            continue;
                        }

                        // Skip content item property
                        if (propertyInfo.Name == "ContentItem")
                        {
                            continue;
                        }

                        propertyInfo.SetValue(targetPart, propertyInfo.GetValue(sourcePart));
                    }
                }
            }
        }
    }
}