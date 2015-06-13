namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Builders;
    using Orchard.ContentManagement.MetaData.Models;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    public abstract class AlterContentPartFieldCmdletBase : TenantCmdlet
    {
        [Alias("cp")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public string ContentPart { get; set; }

        [Alias("cf")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, Position = 2)]
        public string ContentField { get; set; }

        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentPartDefinition ContentPartObject { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = false)]
        public override string Tenant { get; set; }

        protected abstract string GetActionName(string contentFieldName);
        protected abstract void PerformAction(ContentPartDefinitionBuilder builder, string contentFieldName);

        protected override void ProcessRecord(ShellSettings tenant)
        {
            ContentPartDefinition contentPart = this.GetContentPartDefinition(tenant.Name);
            if (contentPart != null)
            {
                string target = "Content Part: " + contentPart.Name + ", Tenant: " + tenant.Name;

                if (this.ShouldProcess(target, this.GetActionName(this.ContentField)))
                {
                    this.UsingWorkContextScope(
                        tenant.Name,
                        scope => scope.Resolve<IContentDefinitionManager>()
                            .AlterPartDefinition(
                                contentPart.Name,
                                builder => this.PerformAction(builder, this.ContentField)));
                }
            }
        }

        private ContentPartDefinition GetContentPartDefinition(string tenantName)
        {
            if (this.ContentPartObject != null)
            {
                return this.ContentPartObject;
            }

            if (this.ContentPart != null)
            {
                ContentPartDefinition contentPart = this.UsingWorkContextScope(
                    tenantName, 
                    scope => scope.Resolve<IContentDefinitionManager>().GetPartDefinition(this.ContentPart));

                if (contentPart == null)
                {
                    this.NotifyFailedToFindContentPart(this.ContentPart, tenantName);
                }

                return contentPart;
            }

            this.NotifyFailedToFindContentPart(string.Empty, tenantName);
            return null;
        }

        private void NotifyFailedToFindContentPart(string name, string tenantName)
        {
            this.WriteError(Error.InvalidArgument(
                "Failed to find content part '" + name + "' in tenant '" + tenantName + "'.",
                "FailedToFindContentPart"));
        }
    }
}