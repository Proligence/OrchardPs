// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardCommand.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Commands.Items 
{
    using System;

    /// <summary>
    /// Represents a legacy Orchard command.
    /// </summary>
    [Serializable]
    public class OrchardCommand 
    {
        /// <summary>
        /// Gets or sets the name (and arguments) of the command.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Gets or sets the command's help text.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Gets or sets the name of the Orchard site which the command belongs to.
        /// </summary>
        public string SiteName { get; set; }
    }
}