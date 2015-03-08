namespace Proligence.PowerShell.Content.Cmdlets
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using Orchard;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Builders;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Core.Contents.Extensions;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Utilities;

    [CmdletAlias("nct")]
    [Cmdlet(VerbsCommon.New, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class NewContentType : OrchardCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the new content type.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display name of the new content type.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the names of the content parts which will be added to the new content type.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string[] Parts { get; set; }

        /// <summary>
        /// Gets or sets the stereotype of the new content type.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string Stereotype { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an instance of the new content type can be created through the UI.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public SwitchParameter Creatable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an instance of the new content type can be listed through the UI.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public SwitchParameter Listable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the new content type supports draft versions.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public SwitchParameter Draftable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the new content type can have custom permissions.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public SwitchParameter Securable { get; set; }

        /// <summary>
        /// Gets or sets the custom settings of the new content type.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public Hashtable Settings { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which the content type will be created.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which content type definitions will be created.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        protected override void ProcessRecord()
        {
            string tenantName = this.GetTenantName();
            
            ShellSettings tenant = this.TenantObject;
            if (tenant == null)
            {
                tenant = this.Resolve<IShellSettingsManager>().LoadSettings()
                    .FirstOrDefault(t => t.Name == tenantName);
            }

            if (tenant != null)
            {
                this.UsingWorkContextScope(
                    tenant.Name,
                    scope => this.CreateContentType(scope, tenantName));
            }
            else
            {
                this.NotifyFailedToFindTenant(tenantName);
            }
        }

        private void CreateContentType(IWorkContextScope scope, string tenantName)
        {
            var contentDefinitionManager = scope.Resolve<IContentDefinitionManager>();

            ContentTypeDefinition contentTypeDefinition = contentDefinitionManager
                .ListTypeDefinitions()
                .FirstOrDefault(cpd => cpd.Name == this.Name);

            if (contentTypeDefinition == null)
            {
                string target = "ContentType: " + this.Name;
                if (this.ShouldProcess(target, "Create"))
                {
                    contentDefinitionManager.AlterTypeDefinition(
                        this.Name,
                        this.AlterContentTypeDefinition);
                }
            }
            else
            {
                this.NotifyExistingContentTypeDefinition(this.Name, tenantName);
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

        private void AlterContentTypeDefinition(ContentTypeDefinitionBuilder builder)
        {
            if (!string.IsNullOrWhiteSpace(this.DisplayName))
            {
                builder.DisplayedAs(this.DisplayName);
            }

            if (!string.IsNullOrWhiteSpace(this.Stereotype))
            {
                builder.WithSetting("Stereotype", this.Stereotype);
            }

            if (this.Creatable.IsPresent)
            {
                builder.Creatable(this.Creatable.ToBool());
            }

            if (this.Listable.IsPresent)
            {
                builder.Listable(this.Listable.ToBool());
            }

            if (this.Draftable.IsPresent)
            {
                builder.Draftable(this.Draftable.ToBool());
            }

            if (this.Securable.IsPresent)
            {
                builder.Securable(this.Securable.ToBool());
            }

            if (this.Parts != null)
            {
                foreach (string partName in this.Parts)
                {
                    builder.WithPart(partName);
                }
            }

            if (this.Settings != null)
            {
                foreach (DictionaryEntry setting in this.Settings)
                {
                    string value = setting.Value != null ? setting.Value.ToString() : null;
                    builder.WithSetting(setting.Key.ToString(), value);
                }
            }
        }

        private void NotifyFailedToFindTenant(string tenantName)
        {
            var exception = new InvalidOperationException("Failed to find tenant '" + tenantName + "'.");
            this.WriteError(exception, "FailedToFindTentant", ErrorCategory.InvalidArgument);
        }

        private void NotifyExistingContentTypeDefinition(string tenantName, string contentTypeName)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture, 
                "The tenant '{0}' already contains a content type with name '{1}'.", 
                tenantName, 
                contentTypeName);

            var exception = new InvalidOperationException(message);
            this.WriteError(exception, "ContentTypeDefinitionExists", ErrorCategory.InvalidArgument);
        }
    }
}