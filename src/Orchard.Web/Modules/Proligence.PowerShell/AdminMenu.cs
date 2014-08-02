﻿using Orchard.Core.Settings;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Proligence.PowerShell {
    [OrchardFeature("Proligence.Powershell.WebConsole")]
    public class AdminMenu : INavigationProvider {
        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("powershell")
                .Add(T("PowerShell"), "11",
                    menu => menu.Action("Console", "Admin", new { area = "Proligence.PowerShell" })
                        .Add(T("Console"), "1.0", item => item.Action("Console", "Admin", new { area = "Proligence.PowerShell" })
                            .LocalNav().Permission(Permissions.ManageSettings)));
        }
    }
}