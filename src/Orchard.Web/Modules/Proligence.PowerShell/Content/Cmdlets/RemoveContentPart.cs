namespace Proligence.PowerShell.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData.Builders;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("rcp")]
    [Cmdlet(VerbsCommon.Remove, "ContentPart", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveContentPart : AlterContentTypePartCmdletBase
    {
        protected override string GetActionName(string contentPartName, string tenantName)
        {
            return "Remove Part: " + contentPartName + ", Tenant: " + tenantName;
        }

        protected override void PerformAction(ContentTypeDefinitionBuilder builder, string contentPartName)
        {
            builder.RemovePart(contentPartName);
        }
    }
}