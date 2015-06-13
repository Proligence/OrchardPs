namespace Proligence.PowerShell.Provider.Handlers
{
    using Orchard.ContentManagement;
    using Orchard.ContentManagement.Handlers;
    using Orchard.Localization;
    using Proligence.PowerShell.Provider.Models;

    public class PowerShellSettingsPartHandler : ContentHandler
    {
        public PowerShellSettingsPartHandler()
        {
            this.T = NullLocalizer.Instance;
            this.Filters.Add(new ActivatingFilter<PowerShellSettingsPart>("Site"));
            this.Filters.Add(new TemplateFilterForPart<PowerShellSettingsPart>("PowerShellSettings", "Parts/PowerShell.Settings", "powershell"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context)
        {
            if (context.ContentItem.ContentType == "Site")
            {
                base.GetItemMetadata(context);
                context.Metadata.EditorGroupInfo.Add(new GroupInfo(this.T("PowerShell")));
            }
        }
    }
}