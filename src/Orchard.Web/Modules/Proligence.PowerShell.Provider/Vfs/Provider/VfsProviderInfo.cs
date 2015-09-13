using System.Management.Automation;

namespace Proligence.PowerShell.Provider.Vfs.Provider {
    /// <summary>
    /// Represents the state of a single instance of the VFS-based PS provider.
    /// </summary>
    public class VfsProviderInfo : ProviderInfo {
        public VfsProviderInfo(ProviderInfo providerInfo)
            : base(providerInfo) {
        }
    }
}