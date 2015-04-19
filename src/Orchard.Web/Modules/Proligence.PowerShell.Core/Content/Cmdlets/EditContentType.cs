namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Collections;
    using System.Management.Automation;
    using System.Text;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Core.Contents.Extensions;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("ect")]
    [Cmdlet(VerbsData.Edit, "ContentType", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EditContentType : AlterContentTypeCmdletBase
    {
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string DisplayName { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public string Stereotype { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Creatable { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Listable { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Draftable { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public SwitchParameter Securable { get; set; }

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

        protected override string GetActionName()
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

        protected override void PerformAction(IContentDefinitionManager contentDefinitionManager)
        {
            contentDefinitionManager.AlterTypeDefinition(
                this.ContentType != null ? this.ContentType.Name : this.Name,
                builder =>
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
                });
        }
    }
}