﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Orchard;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("ecpd")]
    [Cmdlet(VerbsData.Edit, "ContentPartDefinition", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.Medium)]
    public class EditContentPartDefinition : TenantCmdlet {
        private const string SettingPrefix = "ContentPartSettings.";

        [Alias("n")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        [ValidateNotNull]
        [Parameter(ParameterSetName = "ContentPartDefinitionObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentPartDefinition ContentPartDefinition { get; set; }

        [Alias("desc")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartDefinitionObject", Mandatory = false)]
        public string Description { get; set; }

        [Alias("at")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartDefinitionObject", Mandatory = false)]
        public bool? Attachable { get; set; }

        [Alias("se")]
        [Parameter(ParameterSetName = "Default", ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "TenantObject", ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "AllTenants", ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "ContentPartDefinitionObject", ValueFromRemainingArguments = true)]
        public ArrayList Settings { get; set; }

        protected override void ProcessRecord(ShellSettings tenant) {
            this.UsingWorkContextScope(
                tenant.Name,
                scope => {
                    var contentPartDefinition = GetContentPartDefinition(scope);
                    if (contentPartDefinition != null) {
                        UpdateContentPartDefinition(contentPartDefinition);
                        InvokeAlterPartDefinition(contentPartDefinition, scope, tenant.Name);
                    }
                    else {
                        NotifyFailedToFindContentPartDefinition(Name, tenant.Name);
                    }
                });
        }

        private ContentPartDefinition GetContentPartDefinition(IWorkContextScope scope) {
            if (ContentPartDefinition != null) {
                return ContentPartDefinition;
            }

            return scope.Resolve<IContentDefinitionManager>()
                .ListPartDefinitions()
                .FirstOrDefault(cpd => cpd.Name == Name);
        }

        private void UpdateContentPartDefinition(ContentPartDefinition contentPartDefinition) {
            if (Settings != null) {
                ArgumentList settingArgs = ArgumentList.Parse(Settings);
                foreach (KeyValuePair<string, string> setting in settingArgs) {
                    contentPartDefinition.Settings[SettingPrefix + setting.Key] = setting.Value;
                }
            }

            if (Description != null) {
                if (!string.IsNullOrWhiteSpace(Description)) {
                    contentPartDefinition.Settings[SettingPrefix + "Description"] = Description;
                }
                else {
                    contentPartDefinition.Settings[SettingPrefix + "Description"] = null;
                }
            }

            if (Attachable != null) {
                if (Attachable.Value) {
                    contentPartDefinition.Settings[SettingPrefix + "Attachable"] = "True";
                }
                else {
                    contentPartDefinition.Settings[SettingPrefix + "Attachable"] = null;
                }
            }
        }

        private void InvokeAlterPartDefinition(ContentPartDefinition definition, IWorkContextScope scope,
            string tenantName) {
            string target = "Content Part: " + definition.Name + ", Tenant: " + tenantName;
            string action = "Set " + string.Join(", ", definition.Settings.Select(x => x.Key + " = '" + x.Value + "'"));

            if (ShouldProcess(target, action)) {
                if (definition.Settings != null) {
                    scope.Resolve<IContentDefinitionManager>().AlterPartDefinition(
                        definition.Name,
                        part => {
                            foreach (KeyValuePair<string, string> setting in definition.Settings) {
                                part.WithSetting(setting.Key, setting.Value);
                            }
                        });
                }
            }
        }

        private void NotifyFailedToFindContentPartDefinition(string tenantName, string contentPartName) {
            var message = string.Format(
                CultureInfo.CurrentCulture,
                "Failed to find content part definition '{0}' for tenant '{1}'.",
                contentPartName,
                tenantName);

            WriteError(Error.InvalidArgument(message, "FailedToFindContentPartDefinition"));
        }
    }
}