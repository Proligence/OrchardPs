namespace Proligence.PowerShell.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData.Builders;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("acf")]
    [Cmdlet(VerbsCommon.Add, "ContentField", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class AddContentField : AlterContentPartFieldCmdletBase
    {
        protected override string GetActionName(string contentFieldName, string contentPartName, string tenantName)
        {
            return "Add Field: " + contentFieldName + ", Part: " + contentPartName + ", Tenant: " + tenantName;
        }

        protected override void PerformAction(ContentPartDefinitionBuilder builder, string contentFieldName)
        {
            builder.WithField(contentFieldName);
        }
    }
}