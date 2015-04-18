namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using Orchard;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [CmdletAlias("ecpd")]
    [Cmdlet(VerbsData.Edit, "ContentPartDefinition", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.Medium)]
    public class EditContentPartDefinition : OrchardCmdlet
    {
        private const string SettingPrefix = "ContentPartSettings.";
        
        /// <summary>
        /// Gets or sets the name of the content part to edit.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content part definition to edit.
        /// </summary>
        [ValidateNotNull]
        [Parameter(ParameterSetName = "ContentPartDefinitionObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentPartDefinition ContentPartDefinition { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant which contains the edited content part definition.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the Orchard tenant for which content part definitions will be edited.
        /// </summary>
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, ValueFromPipeline = true)]
        public ShellSettings TenantObject { get; set; }

        /// <summary>
        /// Gets or sets the new description of the content part.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartDefinitionObject", Mandatory = false)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the new value of the content part's attachable flag.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartDefinitionObject", Mandatory = false)]
        public bool? Attachable { get; set; }

        /// <summary>
        /// Gets or sets the content part settings to update.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Parameter(ParameterSetName = "Default", ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "TenantObject", ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "ContentPartDefinitionObject", ValueFromRemainingArguments = true)]
        public ArrayList Settings { get; set; }

        protected override void ProcessRecord()
        {
            string tenantName = null;
            string contentPartName = null;
            if (this.ParameterSetName == "Default")
            {
                contentPartName = this.Name;
                tenantName = this.GetTenantName();
            }

            if (contentPartName != null)
            {
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

                            ContentPartDefinition contentPartDefinition = contentDefinitionManager
                                .ListPartDefinitions()
                                .FirstOrDefault(cpd => cpd.Name == contentPartName);

                            if (contentPartDefinition != null)
                            {
                                this.UpdateContentPartDefinition(contentPartDefinition);
                                this.InvokeAlterPartDefinition(contentPartDefinition, scope);
                            }
                            else
                            {
                                this.NotifyFailedToFindContentPartDefinition(contentPartName, tenantName);
                            }
                        });
                }
                else
                {
                    this.WriteError(Error.FailedToFindTenant(tenantName));
                }
            }
            else
            {
                this.NotifyFailedToFindContentPartDefinition(null, tenantName);
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

        private void UpdateContentPartDefinition(ContentPartDefinition contentPartDefinition)
        {
            if (this.Settings != null)
            {
                ArgumentList settingArgs = ArgumentList.Parse(this.Settings);
                foreach (KeyValuePair<string, string> setting in settingArgs)
                {
                    contentPartDefinition.Settings[SettingPrefix + setting.Key] = setting.Value;
                }
            }

            if (this.Description != null)
            {
                if (!string.IsNullOrWhiteSpace(this.Description))
                {
                    contentPartDefinition.Settings[SettingPrefix + "Description"] = this.Description;
                }
                else
                {
                    contentPartDefinition.Settings[SettingPrefix + "Description"] = null;
                }
            }

            if (this.Attachable != null)
            {
                if (this.Attachable.Value)
                {
                    contentPartDefinition.Settings[SettingPrefix + "Attachable"] = "True";
                }
                else
                {
                    contentPartDefinition.Settings[SettingPrefix + "Attachable"] = null;
                }
            }
        }

        private void InvokeAlterPartDefinition(ContentPartDefinition definition, IWorkContextScope scope)
        {
            string target = "ContentPart: " + definition.Name;
            string action = "Set " + string.Join(", ", definition.Settings.Select(x => x.Key + " = '" + x.Value + "'"));
            if (this.ShouldProcess(target, action))
            {
                if (definition.Settings != null)
                {
                    scope.Resolve<IContentDefinitionManager>().AlterPartDefinition(
                        definition.Name,
                        part =>
                        {
                            foreach (KeyValuePair<string, string> setting in definition.Settings)
                            {
                                part.WithSetting(setting.Key, setting.Value);
                            }
                        });
                }
            }
        }

        private void NotifyFailedToFindContentPartDefinition(string tenantName, string contentPartName)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "Failed to find content part definition '{0}' for tenant '{1}'.",
                contentPartName,
                tenantName);

            this.WriteError(Error.InvalidArgument(message, "FailedToFindContentPartDefinition"));
        }
    }
}