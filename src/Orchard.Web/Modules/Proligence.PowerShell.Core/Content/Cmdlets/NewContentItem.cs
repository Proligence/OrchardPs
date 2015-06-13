namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using System.Text;
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Core.Content.Nodes;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("ncit")]
    [Cmdlet(VerbsCommon.New, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class NewContentItem : TenantCmdlet
    {
        [Alias("ct")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public string ContentType { get; set; }

        [Alias("d")]
        [Parameter(ParameterSetName = "Default")]
        [Parameter(ParameterSetName = "TenantObject")]
        [Parameter(ParameterSetName = "AllTenants")]
        [Parameter(ParameterSetName = "ContentTypeObject")]
        public SwitchParameter Draft { get; set; }

        [Alias("p")]
        [Parameter(ParameterSetName = "Default")]
        [Parameter(ParameterSetName = "TenantObject")]
        [Parameter(ParameterSetName = "AllTenants")]
        [Parameter(ParameterSetName = "ContentTypeObject")]
        public SwitchParameter Published { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public override string Tenant { get; set; }

        [ValidateNotNull]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentTypeObject { get; set; }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            var contentType = this.ContentTypeObject != null ? this.ContentTypeObject.Name : this.ContentType;
            var target = "Content Type: " + contentType + ", Tenant: " + tenant.Name;
            
            if (this.ShouldProcess(target, this.GetAction()))
            {
                this.UsingWorkContextScope(
                    tenant.Name,
                    scope =>
                    {
                        var contentManager = scope.Resolve<IContentManager>();
                        var contentItem = contentManager.New(contentType);

                        if (this.Published.ToBool())
                        {
                            contentManager.Create(contentItem, VersionOptions.Published);
                        }
                        else if (this.Draft.ToBool())
                        {
                            contentManager.Create(contentItem, VersionOptions.Draft);
                        }

                        this.WriteObject(ContentItemNode.BuildPSObject(contentItem));
                    });
            }
        }

        private string GetAction()
        {
            var builder = new StringBuilder("Create");

            if (this.Published.ToBool())
            {
                builder.Append(" Published");
            }
            else if (this.Draft.ToBool())
            {
                builder.Append(" Draft");
            }

            return builder.ToString();
        }
    }
}