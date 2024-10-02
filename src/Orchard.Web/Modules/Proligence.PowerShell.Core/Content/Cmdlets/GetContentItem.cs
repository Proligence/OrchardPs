using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Core.Content.Nodes;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("gcit")]
    [Cmdlet(VerbsCommon.Get, "ContentItem", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetContentItem : TenantCmdlet {
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 1)]
        public int? Id { get; set; }

        [Alias("ct")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string ContentType { get; set; }

        [Alias("v")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public int? Version { get; set; }

        [Alias("vo")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public VersionOptionsEnum? VersionOptions { get; set; }

        protected override void ProcessRecord(ShellSettings tenant) {
            this.UsingWorkContextScope(
                tenant.Name,
                scope => {
                    foreach (ContentItem item in GetContentItems(scope.Resolve<IContentManager>())) {
                        if (item != null) {
                            WriteObject(ContentItemNode.BuildPSObject(item));
                        }
                    }
                });
        }

        private IEnumerable<ContentItem> GetContentItems(IContentManager contentManager) {
            if (Id != null) {
                var id = Id.Value;
                var versionOptions = GetVersionOptions();
                if (versionOptions != null) {
                    yield return contentManager.Get(id, versionOptions);
                }
                else {
                    yield return contentManager.Get(id, Orchard.ContentManagement.VersionOptions.Latest);
                }
            }
            else {
                foreach (var contentItem in QueryContentItems(contentManager)) {
                    yield return contentItem;
                }
            }
        }

        private IEnumerable<ContentItem> QueryContentItems(IContentManager contentManager) {
            IContentQuery<ContentItem> query;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (!string.IsNullOrEmpty(ContentType)) {
                query = contentManager.Query(ContentType).WithQueryHintsFor(ContentType);
            }
            else {
                query = contentManager.Query();
            }

            var versionOptions = GetVersionOptions();
            query.ForVersion(versionOptions ?? Orchard.ContentManagement.VersionOptions.Latest);

            return query.List();
        }

        private VersionOptions GetVersionOptions() {
            if (VersionOptions != null) {
                return VersionOptions.Value.ToVersionOptions();
            }

            if (Version != null) {
                return Orchard.ContentManagement.VersionOptions.Number(Version.Value);
            }

            return null;
        }
    }
}