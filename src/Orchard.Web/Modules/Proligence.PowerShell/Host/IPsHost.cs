namespace Proligence.PowerShell.Host
{
    using Orchard;
    using Orchard.Management.PsProvider;

    /// <summary>
    /// Encapsulates the logic of hosting the PowerShell engine inside Orchard.
    /// </summary>
    public interface IPsHost : ISingletonDependency
    {
        /// <summary>
        /// Gets a value indicating whether the PowerShell host is initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the PowerShell snap-in which discovers Orchard PowerShell provider, cmdlets, etc.
        /// </summary>
        OrchardPsSnapIn SnapIn { get; }

        /// <summary>
        /// Initializes the PowerShell host.
        /// </summary>
        void Initialize();
    }
}