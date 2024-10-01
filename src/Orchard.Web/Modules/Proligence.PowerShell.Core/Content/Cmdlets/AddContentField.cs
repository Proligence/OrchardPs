using System.Management.Automation;
using Orchard.ContentManagement.MetaData.Builders;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("acf")]
    [Cmdlet(VerbsCommon.Add, "ContentField", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class AddContentField : AlterContentPartFieldCmdletBase {
        protected override string GetActionName(string contentFieldName) {
            return "Add Field: " + contentFieldName;
        }

        protected override void PerformAction(ContentPartDefinitionBuilder builder, string contentFieldName) {
            builder.WithField(contentFieldName);
        }
    }
}