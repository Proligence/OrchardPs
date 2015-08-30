namespace Proligence.PowerShell.Core.Content.Cmdlets
{
    using System.Management.Automation;
    using System.Xml.Linq;
    using Orchard.ContentManagement;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("icit")]
    [Cmdlet(VerbsData.Import, "ContentItem", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class ImportContentItem : TenantCmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, ValueFromPipeline = true, Position = 1)]
        public string Xml { get; set; }

        protected override void ProcessRecord(ShellSettings tenant)
        {
            this.UsingWorkContextScope(
                tenant.Name,
                scope =>
                {
                    var xmlRoot = XElement.Parse(this.Xml);
                    string targetData = xmlRoot.ToString(SaveOptions.DisableFormatting);
                    if (targetData.Length > 50)
                    {
                        targetData = targetData.Substring(0, 47) + "...";
                    }

                    if (this.ShouldProcess("Tenant: " + tenant.Name + ", Data: " + targetData, "Import"))
                    {
                        var contentManager = scope.Resolve<IContentManager>();
                        var session = new ImportContentSession(contentManager);
                        contentManager.Import(xmlRoot, session);
                    }
                });
        }
    }
}