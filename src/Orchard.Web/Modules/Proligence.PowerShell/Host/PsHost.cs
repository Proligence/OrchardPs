﻿namespace Proligence.PowerShell.Host
{
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Encapsulates the logic of hosting the PowerShell engine inside Orchard.
    /// </summary>
    public class PsHost : IPsHost
    {
        /// <summary>
        /// Gets a value indicating whether the PowerShell host is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the PowerShell snap-in which discovers Orchard PowerShell provider, cmdlets, etc.
        /// </summary>
        public OrchardPsSnapIn SnapIn { get; private set; }

        /// <summary>
        /// Initializes the PowerShell host.
        /// </summary>
        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                this.SnapIn = new OrchardPsSnapIn();
                this.SnapIn.Initialize();
                this.IsInitialized = true;
            }
        }
    }
}