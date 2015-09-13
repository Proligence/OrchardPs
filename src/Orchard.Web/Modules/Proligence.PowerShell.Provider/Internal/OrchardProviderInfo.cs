using System.Management.Automation;
using Proligence.PowerShell.Provider.Vfs.Provider;

namespace Proligence.PowerShell.Provider.Internal {
    /// <summary>
    /// Represents the state of a single instance of the Orchard PS provider.
    /// </summary>
    public class OrchardProviderInfo : VfsProviderInfo {
        internal OrchardProviderInfo(ProviderInfo providerInfo)
            : base(providerInfo) {
        }
    }
}