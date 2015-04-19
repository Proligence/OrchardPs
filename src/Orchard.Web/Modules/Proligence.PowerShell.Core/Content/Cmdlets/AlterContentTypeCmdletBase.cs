namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    public abstract class AlterContentTypeCmdletBase : TenantCmdlet
    {
        private readonly bool failIfExists;
        private readonly bool failIfDoesNotExist;

        protected AlterContentTypeCmdletBase(bool failIfExists = false, bool faileIfDoesNotExist = false)
        {
            this.failIfExists = failIfExists;
            this.failIfDoesNotExist = faileIfDoesNotExist;
        }

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 1)]
        public string Name { get; set; }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            this.UsingWorkContextScope(
                tenant.Name,
                scope =>
                {
                    var contentDefinitionManager = scope.Resolve<IContentDefinitionManager>();

                    ContentTypeDefinition contentTypeDefinition = contentDefinitionManager
                        .ListTypeDefinitions()
                        .FirstOrDefault(cpd => cpd.Name == this.Name);

                    if (this.failIfExists && (contentTypeDefinition != null))
                    {
                        this.NotifyContentTypeExists(this.Name, tenant.Name);
                        return;
                    }

                    if (this.failIfDoesNotExist && (contentTypeDefinition == null))
                    {
                        this.NotifyContentTypeDoesNotExist(this.Name, tenant.Name);
                        return;
                    }

                    string target = "ContentType: " + this.Name + ", Tenant: " + tenant.Name;
                    if (this.ShouldProcess(target, this.GetActionName()))
                    {
                        this.PerformAction(contentDefinitionManager);
                    }
                });
        }

        protected abstract string GetActionName();
        protected abstract void PerformAction(IContentDefinitionManager contentDefinitionManager);

        private void NotifyContentTypeExists(string tenantName, string contentTypeName)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture, 
                "The tenant '{0}' already contains a content type with name '{1}'.", 
                tenantName, 
                contentTypeName);

            this.WriteError(Error.InvalidOperation(message, "ContentTypeExists"));
        }

        private void NotifyContentTypeDoesNotExist(string tenantName, string contentTypeName)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "The tenant '{0}' does not contain a content type with name '{1}'.",
                tenantName,
                contentTypeName);

            this.WriteError(Error.InvalidOperation(message, "ContentTypeDoesNotExist"));
        }
    }
}