using System.Management.Automation;
using Orchard.ContentManagement.MetaData.Builders;
using Proligence.PowerShell.Provider;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    [CmdletAlias("rcp")]
    [Cmdlet(VerbsCommon.Remove, "ContentPart", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveContentPart : AlterContentTypePartCmdletBase {
        protected override string GetActionName(string contentPartName) {
            return "Remove Part: " + contentPartName;
        }

        protected override void PerformAction(ContentTypeDefinitionBuilder builder, string contentPartName) {
            builder.RemovePart(contentPartName);
        }
    }
}