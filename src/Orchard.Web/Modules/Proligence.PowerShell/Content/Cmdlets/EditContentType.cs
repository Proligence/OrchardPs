namespace Proligence.PowerShell.Content.Cmdlets
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Builders;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Core.Contents.Extensions;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Utilities;

    [CmdletAlias("ect")]
    [Cmdlet(VerbsData.Edit, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EditContentType : OrchardCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the content type to edit.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the new display name of the content type.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the new stereotype of the content type.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string Stereotype { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an instance of the content type can be created through the UI.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Creatable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an instance of the content type can be listed through the UI.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Listable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content type supports draft versions.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Draftable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content type can have custom permissions.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Securable { get; set; }

        /// <summary>
        /// Gets or sets the new custom settings of the content type.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public Hashtable Settings { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant for which the content type will be edited.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which the content type definition will be edited.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        /// <summary>
        /// Gets or sets the content type to edit.
        /// </summary>
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentType { get; set; }

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
                        string contentTypeName = this.ContentType != null ? this.ContentType.Name : this.Name;

                        ContentTypeDefinition contentTypeDefinition = contentDefinitionManager
                            .ListTypeDefinitions()
                            .FirstOrDefault(cpd => cpd.Name == contentTypeName);

                        if (contentTypeDefinition != null)
                        {
                            string target = "ContentType: " + contentTypeName;
                            if (this.ShouldProcess(target, this.GetAction()))
                            {
                                contentDefinitionManager.AlterTypeDefinition(
                                    contentTypeName,
                                    this.AlterContentTypeDefinition);
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
                this.NotifyFailedToFindTenant(tenantName);
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
                return this.Tenant;
            }

            if (this.TenantObject != null)
            {
                return this.TenantObject.Name;
            }

            return this.GetCurrentTenantName() ?? "Default";
        }

        private string GetAction()
        {
            var builder = new StringBuilder("Edit: ");

            if (this.MyInvocation.BoundParameters.ContainsKey("DisplayName"))
            {
                builder.Append("DisplayName='" + this.DisplayName + "' ");
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Stereotype"))
            {
                builder.Append("Stereotype='" + this.Stereotype + "' ");
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Creatable"))
            {
                builder.Append("Creatable='" + this.Creatable.ToBool() + "' ");
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Listable"))
            {
                builder.Append("Listable='" + this.Listable.ToBool() + "' ");
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Draftable"))
            {
                builder.Append("Draftable='" + this.Draftable.ToBool() + "' ");
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Securable"))
            {
                builder.Append("Securable='" + this.Securable.ToBool() + "' ");
            }

            if (this.Settings != null)
            {
                foreach (DictionaryEntry setting in this.Settings)
                {
                    string value = setting.Value != null ? "'" + setting.Value + "'" : "$null";
                    builder.Append(setting.Key + "=" + value + " ");
                }
            }

            return builder.ToString().Trim();
        }

        private void AlterContentTypeDefinition(ContentTypeDefinitionBuilder builder)
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("DisplayName"))
            {
                builder.DisplayedAs(this.DisplayName);
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Stereotype"))
            {
                builder.WithSetting("Stereotype", this.Stereotype);
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Creatable"))
            {
                builder.Creatable(this.Creatable.ToBool());
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Listable"))
            {
                builder.Listable(this.Listable.ToBool());
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Draftable"))
            {
                builder.Draftable(this.Draftable.ToBool());
            }

            if (this.MyInvocation.BoundParameters.ContainsKey("Securable"))
            {
                builder.Securable(this.Securable.ToBool());
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

        private void NotifyContentTypeDefinitionNotFound(string tenantName, string contentTypeName)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "The tenant '{0}' does not contain a content type with name '{1}'.",
                tenantName,
                contentTypeName);

            var exception = new InvalidOperationException(message);
            this.WriteError(exception, "ContentTypeDefinitionNotFound", ErrorCategory.ObjectNotFound);
        }
    }
}