namespace Proligence.PowerShell.Content.Cmdlets
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;

    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;

    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements the <c>Edit-ContentPartDefinition</c> cmdlet.
    /// </summary>
    [CmdletAlias("ecpd")]
    [Cmdlet(VerbsData.Edit, "ContentPartDefinition", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.Medium)]
    public class EditContentPartDefinition : OrchardCmdlet
    {
        private const string SettingPrefix = "ContentPartSettings.";
        private ITenantAgent tenantAgent;
        private IContentAgent contentAgent;

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

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
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
                ShellSettings tenant = this.tenantAgent.GetTenant(tenantName);
                if (tenant != null)
                {
                    ContentPartDefinition contentPartDefinition = this.contentAgent
                        .GetContentPartDefinitions(tenantName)
                        .FirstOrDefault(cpd => cpd.Name == contentPartName);

                    if (contentPartDefinition != null)
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

                        this.InvokeUpdateContentPartDefinition(contentPartDefinition);
                    }
                    else
                    {
                        this.NotifyFailedToFindContentPartDefinition(contentPartName, tenantName);
                    }
                }
                else
                {
                    this.NotifyFailedToFindTenant(tenantName);
                }
            }
            else
            {
                this.NotifyFailedToFindContentPartDefinition(contentPartName, tenantName);
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

        private void NotifyFailedToFindContentPartDefinition(string tenantName, string contentPartName)
        {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "Failed to find content part definition '{0}' for tenant '{1}'.",
                contentPartName,
                tenantName);

            var exception = new InvalidOperationException(message);
            this.WriteError(exception, "FailedToFindContentPartDefinition", ErrorCategory.InvalidArgument);
        }

        private void NotifyFailedToFindTenant(string tenantName)
        {
            var exception = new InvalidOperationException("Failed to find tenant '" + tenantName + "'.");
            this.WriteError(exception, "FailedToFindTentant", ErrorCategory.InvalidArgument);
        }

        private void InvokeUpdateContentPartDefinition(ContentPartDefinition definition) {
            string target = "ContentPart: " + definition.Name;
            string action = "Set " + string.Join(", ", definition.Settings.Select(x => x.Key + " = '" + x.Value + "'"));
            if (this.ShouldProcess(target, action))
            {
                try
                {
                    this.contentAgent.UpdateContentPartDefinition(definition);
                }
                catch (ArgumentException ex)
                {
                    this.WriteError(ex, "FailedToUpdateContentPartDefinition", ErrorCategory.InvalidArgument);
                }
            }
        }
    }
}