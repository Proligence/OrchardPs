using System.Management.Automation;

namespace Orchard.Management.PsProvider {
    public class OrchardDriveParameters {
        [Parameter(Mandatory = true)]
        public string OrchardRoot { get; set; }
    }
}