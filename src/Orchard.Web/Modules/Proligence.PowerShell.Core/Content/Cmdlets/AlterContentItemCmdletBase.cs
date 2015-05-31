namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Globalization;
    using System.Management.Automation;
    using Orchard.ContentManagement;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    public abstract class AlterContentItemCmdletBase : TenantCmdlet
    {
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public int? Id { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public virtual VersionOptionsEnum? VersionOptions { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public virtual int? Version { get; set; }

        [Parameter(ParameterSetName = "ContentItemObject", Mandatory = true, ValueFromPipeline = true, Position = 1)]
        public ContentItem ContentItem { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentItemObject", Mandatory = false)]
        public override string Tenant { get; set; }

        protected abstract string GetActionName();
        protected abstract void PerformAction(IContentManager contentManager, ContentItem contentItem);

        protected override void ProcessRecord(ShellSettings tenant)
        {
            this.UsingWorkContextScope(tenant.Name, scope =>
            {
                var contentManager = scope.Resolve<IContentManager>();
                var contentItem = this.GetContentItem(contentManager, tenant.Name);
                if (contentItem != null)
                {
                    var target = string.Format(
                        CultureInfo.InvariantCulture,
                        "Content Item: {0}, Version: {1}, Tenant: {2}",
                        contentItem.Id,
                        contentItem.Version,
                        tenant.Name);
                        
                    if (this.ShouldProcess(target, this.GetActionName()))
                    {
                        this.PerformAction(contentManager, contentItem);
                    }
                }
            });
        }

        private ContentItem GetContentItem(IContentManager contentManager, string tenantName)
        {
            var id = 0;
            var versionOptions = Orchard.ContentManagement.VersionOptions.Latest;

            if (this.ContentItem != null)
            {
                id = this.ContentItem.Id;
                versionOptions = Orchard.ContentManagement.VersionOptions.Number(this.ContentItem.Version);
            }

            if (this.Id != null)
            {
                id = this.Id.Value;
                versionOptions = Orchard.ContentManagement.VersionOptions.Latest;
            }

            if (this.VersionOptions != null)
            {
                versionOptions = this.GetVersionOptions();
            }

            if (this.Version != null)
            {
                versionOptions = Orchard.ContentManagement.VersionOptions.Number(this.Version.Value);
            }

            var contentItem = contentManager.Get(id, versionOptions);
            if (contentItem == null)
            {
                this.WriteError(Error.InvalidArgument(
                    "Failed to find content item with ID '" + id + "' in tenant '" + tenantName + "'.",
                    "FailedToFindContentItem"));
            }

            return contentItem;
        }

        private VersionOptions GetVersionOptions()
        {
            if (this.VersionOptions != null)
            {
                return this.VersionOptions.Value.ToVersionOptions();
            }

            if (this.Version != null)
            {
                return Orchard.ContentManagement.VersionOptions.Number(this.Version.Value);
            }

            return null;
        }
    }
}