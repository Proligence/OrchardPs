namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Collections.Generic;
    using System.Management.Automation;
    using Orchard.ContentManagement;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Core.Content.Nodes;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("gcit")]
    [Cmdlet(VerbsCommon.Get, "ContentItem", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetContentItem : TenantCmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 1)]
        public string ContentType { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public int? Id { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public int? Version { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public VersionOptionsEnum? VersionOptions { get; set; }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            this.UsingWorkContextScope(
                tenant.Name,
                scope =>
                {
                    foreach (ContentItem item in this.GetContentItems(scope.Resolve<IContentManager>()))
                    {
                        if (item != null)
                        {
                            this.WriteObject(ContentItemNode.BuildPSObject(item));
                        }
                    }
                });
        }

        private IEnumerable<ContentItem> GetContentItems(IContentManager contentManager)
        {
            if (this.Id != null)
            {
                var versionOptions = this.GetVersionOptions();
                if (versionOptions != null)
                {
                    yield return contentManager.Get(this.Id.Value, versionOptions);
                }
                else
                {
                    yield return contentManager.Get(this.Id.Value);
                }
            }
            else
            {
                foreach (var contentItem in this.QueryContentItems(contentManager))
                {
                    yield return contentItem;
                }
            }
        }

        private IEnumerable<ContentItem> QueryContentItems(IContentManager contentManager)
        {
            IContentQuery<ContentItem> query;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                query = contentManager.Query(this.ContentType);
            }
            else
            {
                query = contentManager.Query();
            }

            var versionOptions = this.GetVersionOptions();
            if (versionOptions != null)
            {
                query.ForVersion(versionOptions);
            }

            return query.List();
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