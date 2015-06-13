namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [CmdletAlias("gcp")]
    [Cmdlet(VerbsCommon.Get, "ContentPart", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetContentPart : TenantCmdlet
    {
        [Alias("ct")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public string ContentType { get; set; }

        [Alias("n")]
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = false, Position = 2)]
        public string Name { get; set; }

        [ValidateNotNull]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentTypeObject { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        public override string Tenant { get; set; }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            ContentTypeDefinition contentType = this.GetContentTypeDefinition(tenant.Name);
            if (contentType != null)
            {
                foreach (ContentTypePartDefinition part in contentType.Parts)
                {
                    if (!string.IsNullOrEmpty(this.Name))
                    {
                        if (!part.PartDefinition.Name.WildcardEquals(this.Name))
                        {
                            continue;
                        }
                    }

                    this.WriteObject(part);
                }
            }
        }

        private ContentTypeDefinition GetContentTypeDefinition(string tenantName)
        {
            if (this.ContentTypeObject != null)
            {
                return this.ContentTypeObject;
            }

            if (this.ContentType != null)
            {
                ContentTypeDefinition contentType = this.UsingWorkContextScope(
                    tenantName,
                    scope => scope.Resolve<IContentDefinitionManager>().GetTypeDefinition(this.ContentType));

                if (contentType == null)
                {
                    this.WriteError(Error.InvalidArgument(
                        "Failed to find content type '" + this.ContentType + "' in tenant '" + tenantName + "'.",
                        "FailedToFindTentant"));
                }

                return contentType;
            }

            return null;
        }
    }
}