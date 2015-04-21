﻿namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using Orchard.ContentManagement.MetaData.Builders;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("rcf")]
    [Cmdlet(VerbsCommon.Remove, "ContentField", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class RemoveContentField : AlterContentPartFieldCmdletBase
    {
        protected override string GetActionName(string contentFieldName)
        {
            return "Remove Field: " + contentFieldName;
        }

        protected override void PerformAction(ContentPartDefinitionBuilder builder, string contentFieldName)
        {
            builder.RemoveField(contentFieldName);
        }
    }
}