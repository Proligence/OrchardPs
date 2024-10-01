using System.Collections;
using System.Management.Automation;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("nct")]
    [Cmdlet(VerbsCommon.New, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class NewContentType : AlterContentTypeCmdletBase {
        // ReSharper disable once RedundantArgumentNameForLiteralExpression
        public NewContentType()
            : base(failIfExists: true) {
        }

        [Alias("dn")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string DisplayName { get; set; }

        [Alias("p")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string[] Parts { get; set; }

        [Alias("st")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Stereotype { get; set; }

        [Alias("c")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Creatable { get; set; }

        [Alias("l")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Listable { get; set; }

        [Alias("d")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Draftable { get; set; }

        [Alias("s")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Securable { get; set; }

        [Alias("se")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public Hashtable Settings { get; set; }

        protected override string GetActionName() {
            return "Create";
        }

        protected override void PerformAction(IContentDefinitionManager contentDefinitionManager) {
            contentDefinitionManager.AlterTypeDefinition(
                Name,
                builder => {
                    if (!string.IsNullOrWhiteSpace(DisplayName)) {
                        builder.DisplayedAs(DisplayName);
                    }

                    if (!string.IsNullOrWhiteSpace(Stereotype)) {
                        builder.WithSetting("Stereotype", Stereotype);
                    }

                    if (Creatable.IsPresent) {
                        builder.Creatable(Creatable.ToBool());
                    }

                    if (Listable.IsPresent) {
                        builder.Listable(Listable.ToBool());
                    }

                    if (Draftable.IsPresent) {
                        builder.Draftable(Draftable.ToBool());
                    }

                    if (Securable.IsPresent) {
                        builder.Securable(Securable.ToBool());
                    }

                    if (Parts != null) {
                        foreach (string partName in Parts) {
                            builder.WithPart(partName);
                        }
                    }

                    if (Settings != null) {
                        foreach (DictionaryEntry setting in Settings) {
                            string value = setting.Value != null
                                ? setting.Value.ToString()
                                : null;
                            builder.WithSetting(setting.Key.ToString(), value);
                        }
                    }
                });
        }
    }
}