namespace Proligence.PowerShell.Provider.Vfs.Provider
{
    using System.Management.Automation;

    /// <summary>
    /// Represents the state of a single instance of the VFS-based PS provider.
    /// </summary>
    public class VfsProviderInfo : ProviderInfo 
    {
        public VfsProviderInfo(ProviderInfo providerInfo) 
            : base(providerInfo)
        {
        }
    }
}