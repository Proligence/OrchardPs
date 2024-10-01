using System.Collections;
using System.Management.Automation;
using System.Text;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Extensions;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("ect")]
    [Cmdlet(VerbsData.Edit, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EditContentType : AlterContentTypeCmdletBase {
        [Alias("n")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 1)]
        public override string Name {
            get {
                return ContentType != null
                    ? ContentType.Name
                    : base.Name;
            }
            set {
                base.Name = value;
            }
        }

        [Alias("dn")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string DisplayName { get; set; }

        [Alias("st")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string Stereotype { get; set; }

        [Alias("c")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Creatable { get; set; }

        [Alias("l")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Listable { get; set; }

        [Alias("d")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Draftable { get; set; }

        [Alias("s")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Securable { get; set; }

        [Alias("se")]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public Hashtable Settings { get; set; }

        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentType { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public override string Tenant { get; set; }

        protected override string GetActionName() {
            var builder = new StringBuilder("Edit: ");

            if (MyInvocation.BoundParameters.ContainsKey("DisplayName")) {
                builder.Append("DisplayName='" + DisplayName + "' ");
            }

            if (MyInvocation.BoundParameters.ContainsKey("Stereotype")) {
                builder.Append("Stereotype='" + Stereotype + "' ");
            }

            if (MyInvocation.BoundParameters.ContainsKey("Creatable")) {
                builder.Append("Creatable='" + Creatable.ToBool() + "' ");
            }

            if (MyInvocation.BoundParameters.ContainsKey("Listable")) {
                builder.Append("Listable='" + Listable.ToBool() + "' ");
            }

            if (MyInvocation.BoundParameters.ContainsKey("Draftable")) {
                builder.Append("Draftable='" + Draftable.ToBool() + "' ");
            }

            if (MyInvocation.BoundParameters.ContainsKey("Securable")) {
                builder.Append("Securable='" + Securable.ToBool() + "' ");
            }

            if (Settings != null) {
                foreach (DictionaryEntry setting in Settings) {
                    string value = setting.Value != null
                        ? "'" + setting.Value + "'"
                        : "$null";
                    builder.Append(setting.Key + "=" + value + " ");
                }
            }

            return builder.ToString().Trim();
        }

        protected override void PerformAction(IContentDefinitionManager contentDefinitionManager) {
            contentDefinitionManager.AlterTypeDefinition(
                ContentType != null
                    ? ContentType.Name
                    : Name,
                builder => {
                    if (MyInvocation.BoundParameters.ContainsKey("DisplayName")) {
                        builder.DisplayedAs(DisplayName);
                    }

                    if (MyInvocation.BoundParameters.ContainsKey("Stereotype")) {
                        builder.WithSetting("Stereotype", Stereotype);
                    }

                    if (MyInvocation.BoundParameters.ContainsKey("Creatable")) {
                        builder.Creatable(Creatable.ToBool());
                    }

                    if (MyInvocation.BoundParameters.ContainsKey("Listable")) {
                        builder.Listable(Listable.ToBool());
                    }

                    if (MyInvocation.BoundParameters.ContainsKey("Draftable")) {
                        builder.Draftable(Draftable.ToBool());
                    }

                    if (MyInvocation.BoundParameters.ContainsKey("Securable")) {
                        builder.Securable(Securable.ToBool());
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