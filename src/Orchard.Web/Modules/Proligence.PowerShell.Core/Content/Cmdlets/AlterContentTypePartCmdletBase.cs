namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Builders;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;

    public abstract class AlterContentTypePartCmdletBase : OrchardCmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, Position = 1)]
        public string ContentType { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, Position = 2)]
        public string ContentPart { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = false)]
        public string Tenant { get; set; }

        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentTypeObject { get; set; }

        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentPartDefinition ContentPartObject { get; set; }

        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        protected abstract string GetActionName(string contentPartName, string tenantName);

        protected abstract void PerformAction(ContentTypeDefinitionBuilder builder, string contentPartName);

        protected override void ProcessRecord()
        {
            string tenantName = this.GetTenantName();
            if (tenantName != null)
            {
                ContentTypeDefinition contentType = this.GetContentTypeDefinition(tenantName);
                if (contentType != null)
                {
                    string contentPartName = this.GetContentPartName();
                    if (contentPartName != null)
                    {
                        if (this.ShouldProcess(contentType.Name, this.GetActionName(contentPartName, tenantName)))
                        {
                            this.UsingWorkContextScope(
                                tenantName, 
                                scope => scope.Resolve<IContentDefinitionManager>()
                                    .AlterTypeDefinition(
                                        contentType.Name,
                                        builder => this.PerformAction(builder, contentPartName)));
                        }
                    }
                }
            }
        }

        private string GetTenantName()
        {
            if (this.ParameterSetName == "TenantObject")
            {
                return this.TenantObject.Name;
            }

            if (this.Tenant != null)
            {
                if (this.Resolve<IShellSettingsManager>().LoadSettings().All(t => t.Name != this.Tenant))
                {
                    this.NotifyFailedToFindTenant(this.Tenant);
                    return null;
                }

                return this.Tenant;
            }

            if (this.TenantObject != null)
            {
                return this.TenantObject.Name;
            }

            return this.GetCurrentTenantName() ?? "Default";
        }

        private ContentTypeDefinition GetContentTypeDefinition(string tenantName)
        {
            if (this.ContentTypeObject != null)
            {
                return this.ContentTypeObject;
            }

            if (this.ContentType != null)
            {
                ContentTypeDefinition contentType = this.UsingWorkContextScope(
                    tenantName, 
                    scope => scope.Resolve<IContentDefinitionManager>().GetTypeDefinition(this.ContentType));

                if (contentType == null)
                {
                    this.NotifyFailedToFindContentType(this.ContentType, tenantName);
                }

                return contentType;
            }

            this.NotifyFailedToFindContentType(string.Empty, tenantName);
            return null;
        }

        private string GetContentPartName()
        {
            if (this.ParameterSetName == "ContentPartObject")
            {
                return this.ContentPartObject.Name;
            }

            if (this.ContentPart != null)
            {
                return this.ContentPart;
            }

            if (this.ContentPartObject != null)
            {
                return this.ContentPartObject.Name;
            }

            this.NotifyInvalidContentPartName();
            return null;
        }

        private void NotifyFailedToFindTenant(string tenantName)
        {
            var exception = new InvalidOperationException("Failed to find tenant '" + tenantName + "'.");
            this.WriteError(exception, "FailedToFindTentant", ErrorCategory.InvalidArgument);
        }

        private void NotifyFailedToFindContentType(string name, string tenantName)
        {
            var exception = new InvalidOperationException(
                "Failed to find content type '" + name + "' in tenant '" + tenantName + "'.");
            this.WriteError(exception, "FailedToFindTentant", ErrorCategory.InvalidArgument);
        }

        private void NotifyInvalidContentPartName()
        {
            var exception = new InvalidOperationException("Invalid content part specified.");
            this.WriteError(exception, "InvalidContentPart", ErrorCategory.InvalidArgument);
        }
    }
}