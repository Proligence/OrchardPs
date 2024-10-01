using Proligence.PowerShell.Provider;

namespace OrchardPs.Host {
    /// <summary>
    /// Represents Orchard session context.
    /// </summary>
    public class OrchardHostContext {
        /// <summary>
        /// Gets or sets the directory of the Orchard installation.
        /// </summary>
        public string OrchardDirectory { get; set; }

        /// <summary>
        /// Gets or sets the virtual path of the Orchard web application.
        /// </summary>
        public string VirtualPath { get; set; }

        /// <summary>
        /// Gets or sets the working directory of the Orchard web application.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the Orchard host instance.
        /// </summary>
        public OrchardHost OrchardHost { get; set; }

        /// <summary>
        /// Gets or sets the PowerShell session associated with the host.
        /// </summary>
        public IPsSession Session { get; set; }
    }
}