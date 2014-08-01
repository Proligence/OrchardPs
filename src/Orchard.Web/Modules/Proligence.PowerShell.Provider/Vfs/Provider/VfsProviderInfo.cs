namespace Proligence.PowerShell.Provider.Vfs.Provider
{
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;
    using Autofac;

    /// <summary>
    /// Represents the state of a single instance of the VFS-based PS provider.
    /// </summary>
    public class VfsProviderInfo : ProviderInfo 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VfsProviderInfo"/> class.
        /// </summary>
        /// <param name="providerInfo">The <see cref="ProviderInfo"/> object of the PS provider.</param>
        [ExcludeFromCodeCoverage]
        public VfsProviderInfo(ProviderInfo providerInfo) 
            : base(providerInfo)
        {
        }
    }
}