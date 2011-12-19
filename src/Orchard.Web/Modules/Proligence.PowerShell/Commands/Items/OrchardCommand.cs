using System;

namespace Proligence.PowerShell.Commands.Items {
    [Serializable]
    public class OrchardCommand {
        public string CommandName { get; set; }
        public string HelpText { get; set; }
        public string SiteName { get; set; }
    }
}