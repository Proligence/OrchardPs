namespace Proligence.PowerShell.Provider
{
    using System.Management.Automation;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Represents the state of a single instance of the Orchard PS provider.
    /// </summary>
    public class OrchardProviderInfo : VfsProviderInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderInfo"/> class.
        /// </summary>
        /// <param name="providerInfo">The <see cref="ProviderInfo"/> object of the PS provider.</param>
        internal OrchardProviderInfo(ProviderInfo providerInfo) 
            : base(providerInfo)
        {
        }
    }
}