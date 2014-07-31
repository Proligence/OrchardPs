using Orchard.Environment.Extensions;
using Orchard.UI.Resources;

namespace Proligence.PowerShell {
    [OrchardFeature("Proligence.Powershell.WebConsole")]
    public class ResourceManifest : IResourceManifestProvider {
        public void BuildManifests(ResourceManifestBuilder builder) {
            var manifest = builder.Add();

            manifest.DefineStyle("PS_Console").SetUrl("console.css");

            manifest.DefineScript("PS_Knockout").SetCdn("//ajax.aspnetcdn.com/ajax/knockout/knockout-2.2.1.js");
            manifest.DefineScript("jQuery_Console").SetUrl("jquery.console.js").SetDependencies("jQuery");
            manifest.DefineScript("PS_Console").SetUrl("console.js").SetDependencies("jQuery_SignalR", "jQuery_Console", "PS_Knockout");
        }
    }
}
