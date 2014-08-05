namespace Proligence.PowerShell.Provider
{
    using System.Management.Automation;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Represents the state of a single instance of the Orchard PS provider.
    /// </summary>
    public class OrchardProviderInfo : VfsProviderInfo
    {
        internal OrchardProviderInfo(ProviderInfo providerInfo) 
            : base(providerInfo)
        {
        }
    }
}