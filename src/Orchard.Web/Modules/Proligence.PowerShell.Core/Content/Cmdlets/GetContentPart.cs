namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Core.Utilities;
    using Proligence.PowerShell.Provider;
    
    [CmdletAlias("gcp")]
    [Cmdlet(VerbsCommon.Get, "ContentPart", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetContentPart : OrchardCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the content type which content parts will be retrieved.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the content type which content parts will be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentTypeObject { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant to which the content type belongs.
        /// </summary>
        [Parameter(ParameterSetName = "Name", Mandatory = false)]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant to which the content type belongs.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        protected override void ProcessRecord()
        {
            string tenantName = this.GetTenantName();
            if (tenantName != null)
            {
                ContentTypeDefinition contentType = this.GetContentTypeDefinition(tenantName);
                if (contentType != null)
                {
                    foreach (ContentTypePartDefinition part in contentType.Parts)
                    {
                        this.WriteObject(part);
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
    }
}