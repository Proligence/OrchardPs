namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsCommon.Remove, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveTenant : OrchardCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the content type to remove.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content type to remove.
        /// </summary>
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentType { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which the content type will be removed.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which content type will be removed.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        protected override void ProcessRecord()
        {
            string tenantName = this.GetTenantName();

            ShellSettings tenant = this.Resolve<IShellSettingsManager>()
                .LoadSettings()
                .FirstOrDefault(t => t.Name == tenantName);

            if (tenant != null)
            {
                this.UsingWorkContextScope(
                    tenant.Name, 
                    scope =>
                    {
                        var contentDefinitionManager = scope.Resolve<IContentDefinitionManager>();
                        var contentTypeName = this.ContentType != null ? this.ContentType.Name : this.Name;

                        ContentTypeDefinition contentTypeDefinition = contentDefinitionManager
                            .ListTypeDefinitions()
                            .FirstOrDefault(cpd => cpd.Name == contentTypeName);

                        if (contentTypeDefinition != null)
                        {
                            if (this.ShouldProcess("ContentType: " + contentTypeName, "Remove"))
                            {
                                contentDefinitionManager.DeleteTypeDefinition(contentTypeName);
                            }
                        }
                        else
                        {
                            this.NotifyContentTypeDefinitionNotFound(this.Name, tenantName);
                        }
                    });
            }
            else
            {
                this.WriteError(Error.FailedToFindTenant(tenantName));
            }
        }

        private string GetTenantName()
        {
            if (this.Tenant != null)
            {
                return this.Tenant;
            }

            if (this.TenantObject != null)
            {
                return this.TenantObject.Name;
            }

            return this.GetCurrentTenantName() ?? "Default";
        }

        private void NotifyContentTypeDefinitionNotFound(string tenantName, string contentTypeName)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture, 
                "The tenant '{0}' does not contain a content type with name '{1}'.", 
                tenantName, 
                contentTypeName);

            this.WriteError(Error.ObjectNotFound(
                new InvalidOperationException(message),
                "ContentTypeDefinitionNotFound"));
        }
    }
}