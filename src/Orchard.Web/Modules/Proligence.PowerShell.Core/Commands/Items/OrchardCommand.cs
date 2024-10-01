using System;

namespace Proligence.PowerShell.Core.Commands.Items {
    /// <summary>
    /// Represents a legacy Orchard command.
    /// </summary>
    [Serializable]
    public class OrchardCommand {
        /// <summary>
        /// Gets or sets the name (and arguments) of the command.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Gets or sets the command's help text.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets the name of the Orchard tenant which the command belongs to.
        /// </summary>
        public string TenantName { get; set; }
    }
}