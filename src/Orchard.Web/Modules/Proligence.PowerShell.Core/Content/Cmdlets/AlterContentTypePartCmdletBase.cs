namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Builders;
    using Orchard.ContentManagement.MetaData.Models;
    
    /// <summary>
    /// This is the base class for cmdlets which add and remove content parts from a content type.
    /// </summary>
    public abstract class AlterContentTypePartCmdletBase : AlterContentTypeCmdletBase
    {
        protected AlterContentTypePartCmdletBase()
            : base(failIfDoesNotExist: true)
        {
        }

        /// <remarks>
        /// This property is overridden without any <see cref="ParameterAttribute"/>, so it won't be visible as a
        /// cmdlet parameter. Instead, this class exposes the <see cref="ContentType"/> and <see cref="ContentPart"/>
        /// cmdlet parameters (the <c>Name</c> parameter would be confusing, as there are two names - the content type
        /// name and content part name). However, the <see cref="Name"/> property must get/set the content type name,
        /// because the base class relies on this property.
        /// </remarks>>
        public override string Name
        {
            get
            {
                return this.ContentTypeObject != null ? this.ContentTypeObject.Name : this.ContentType;
            }

            set
            {
                this.ContentType = value;
            }
        }

        [Alias("ct")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, Position = 1)]
        public string ContentType { get; set; }

        [Alias("cp")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 2)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, Position = 2)]
        public string ContentPart { get; set; }

        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = false)]
        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = false)]
        public override string Tenant { get; set; }

        [Parameter(ParameterSetName = "ContentTypeObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentTypeDefinition ContentTypeObject { get; set; }

        [Parameter(ParameterSetName = "ContentPartObject", Mandatory = true, ValueFromPipeline = true)]
        public ContentPartDefinition ContentPartObject { get; set; }

        protected abstract string GetActionName(string contentPartName);
        protected abstract void PerformAction(ContentTypeDefinitionBuilder builder, string contentPartName);

        protected override string GetTargetName(string tenantName)
        {
            return "Content Type: " + this.Name + ", Tenant: " + tenantName;
        }

        protected override string GetActionName()
        {
            string contentPartName = this.ContentPartObject != null ? this.ContentPartObject.Name : this.ContentPart;
            return this.GetActionName(contentPartName);
        }

        protected override void PerformAction(IContentDefinitionManager contentDefinitionManager)
        {
            string contentPartName = this.ContentPartObject != null ? this.ContentPartObject.Name : this.ContentPart;

            contentDefinitionManager.AlterTypeDefinition(
                this.Name,
                builder => this.PerformAction(builder, contentPartName));
        }
    }
}