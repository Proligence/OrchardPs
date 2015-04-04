namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Builders;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Core.Utilities;
    using Proligence.PowerShell.Provider;

    public abstract class AlterContentPartFieldCmdletBase : OrchardCmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        public string ContentPart { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, Position = 2)]
        public string ContentField { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = false)]
        public string Tenant { get; set; }

        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentPartDefinition ContentPartObject { get; set; }

        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        protected abstract string GetActionName(string contentFieldName, string contentPartName, string tenantName);

        protected abstract void PerformAction(ContentPartDefinitionBuilder builder, string contentFieldName);

        protected override void ProcessRecord()
        {
            string tenantName = this.GetTenantName();
            if (tenantName != null)
            {
                ContentPartDefinition contentPart = this.GetContentPartDefinition(tenantName);
                if (contentPart != null)
                {
                    string contentFieldName = this.GetContentFieldName();
                    if (contentFieldName != null)
                    {
                        if (this.ShouldProcess(
                            contentPart.Name,
                            this.GetActionName(contentFieldName, contentPart.Name, tenantName)))
                        {
                            this.UsingWorkContextScope(
                                tenantName, 
                                scope => scope.Resolve<IContentDefinitionManager>()
                                    .AlterPartDefinition(
                                        contentPart.Name,
                                        builder => this.PerformAction(builder, contentFieldName)));
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

        private ContentPartDefinition GetContentPartDefinition(string tenantName)
        {
            if (this.ContentPartObject != null)
            {
                return this.ContentPartObject;
            }

            if (this.ContentPart != null)
            {
                ContentPartDefinition contentPart = this.UsingWorkContextScope(
                    tenantName, 
                    scope => scope.Resolve<IContentDefinitionManager>().GetPartDefinition(this.ContentPart));

                if (contentPart == null)
                {
                    this.NotifyFailedToFindContentPart(this.ContentPart, tenantName);
                }

                return contentPart;
            }

            this.NotifyFailedToFindContentPart(string.Empty, tenantName);
            return null;
        }

        private string GetContentFieldName()
        {
            if (this.ContentField != null)
            {
                return this.ContentField;
            }

            this.NotifyInvalidContentFieldName();
            return null;
        }

        private void NotifyFailedToFindTenant(string tenantName)
        {
            var exception = new InvalidOperationException("Failed to find tenant '" + tenantName + "'.");
            this.WriteError(exception, "FailedToFindTentant", ErrorCategory.InvalidArgument);
        }

        private void NotifyFailedToFindContentPart(string name, string tenantName)
        {
            var exception = new InvalidOperationException(
                "Failed to find content part '" + name + "' in tenant '" + tenantName + "'.");
            this.WriteError(exception, "FailedToFindContentPart", ErrorCategory.InvalidArgument);
        }

        private void NotifyInvalidContentFieldName()
        {
            var exception = new InvalidOperationException("Invalid content field specified.");
            this.WriteError(exception, "InvalidContentField", ErrorCategory.InvalidArgument);
        }
    }
}