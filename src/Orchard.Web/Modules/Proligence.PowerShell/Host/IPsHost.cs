namespace Proligence.PowerShell.Host
{
    using Orchard;

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
        /// Initializes the PowerShell host.
        /// </summary>
        void Initialize();
    }
}