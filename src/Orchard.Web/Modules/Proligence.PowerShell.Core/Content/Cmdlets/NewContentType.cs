namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Collections;
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.Core.Contents.Extensions;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("nct")]
    [Cmdlet(VerbsCommon.New, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class NewContentType : AlterContentTypeCmdletBase
    {
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string DisplayName { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string[] Parts { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public string Stereotype { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Creatable { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Listable { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Draftable { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public SwitchParameter Securable { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        public Hashtable Settings { get; set; }

        protected override string GetActionName()
        {
            return "Create";
        }

        protected override void PerformAction(IContentDefinitionManager contentDefinitionManager)
        {
            contentDefinitionManager.AlterTypeDefinition(
                this.Name,
                builder =>
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
                });
        }
    }
}