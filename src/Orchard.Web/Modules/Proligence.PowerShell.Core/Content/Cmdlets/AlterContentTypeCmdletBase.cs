using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    public abstract class AlterContentTypeCmdletBase : TenantCmdlet {
        private readonly bool _failIfExists;
        private readonly bool _failIfDoesNotExist;

        protected AlterContentTypeCmdletBase(bool failIfExists = false, bool failIfDoesNotExist = false) {
            _failIfExists = failIfExists;
            _failIfDoesNotExist = failIfDoesNotExist;
        }

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 1)]
        public virtual string Name { get; set; }

        protected override void ProcessRecord(ShellSettings tenant) {
            this.UsingWorkContextScope(
                tenant.Name,
                scope => {
                    var contentDefinitionManager = scope.Resolve<IContentDefinitionManager>();

                    ContentTypeDefinition contentTypeDefinition = contentDefinitionManager
                        .ListTypeDefinitions()
                        .FirstOrDefault(cpd => cpd.Name == Name);

                    if (_failIfExists && (contentTypeDefinition != null)) {
                        NotifyContentTypeExists(Name, tenant.Name);
                        return;
                    }

                    if (_failIfDoesNotExist && (contentTypeDefinition == null)) {
                        NotifyContentTypeDoesNotExist(Name, tenant.Name);
                        return;
                    }

                    if (ShouldProcess(GetTargetName(tenant.Name), GetActionName())) {
                        PerformAction(contentDefinitionManager);
                    }
                });
        }

        protected virtual string GetTargetName(string tenantName) {
            return "ContentType: " + Name + ", Tenant: " + tenantName;
        }

        protected abstract string GetActionName();
        protected abstract void PerformAction(IContentDefinitionManager contentDefinitionManager);

        private void NotifyContentTypeExists(string tenantName, string contentTypeName) {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "The tenant '{0}' already contains a content type with name '{1}'.",
                tenantName,
                contentTypeName);

            WriteError(Error.InvalidOperation(message, "ContentTypeExists"));
        }

        private void NotifyContentTypeDoesNotExist(string tenantName, string contentTypeName) {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "The tenant '{0}' does not contain a content type with name '{1}'.",
                tenantName,
                contentTypeName);

            WriteError(Error.InvalidOperation(message, "ContentTypeDoesNotExist"));
        }
    }
}