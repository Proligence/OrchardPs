namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData.Builders;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("acp")]
    [Cmdlet(VerbsCommon.Add, "ContentPart", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class AddContentPart : AlterContentTypePartCmdletBase
    {
        protected override string GetActionName(string contentPartName, string tenantName)
        {
            return "Add Part: " + contentPartName + ", Tenant: " + tenantName;
        }

        protected override void PerformAction(ContentTypeDefinitionBuilder builder, string contentPartName)
        {
            builder.WithPart(contentPartName);
        }
    }
}