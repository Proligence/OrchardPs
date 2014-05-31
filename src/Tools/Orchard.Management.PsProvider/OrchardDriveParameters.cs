// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardDriveParameters.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System.Management.Automation;

    /// <summary>
    /// Represents the parameters for creating an new Orchard drive in PowerShell.
    /// </summary>
    public class OrchardDriveParameters 
    {
        /// <summary>
        /// Gets or sets the root directory of the Orchard installation represented by the drive.
        /// </summary>
        [Parameter(Mandatory = true)]
        public string OrchardRoot { get; set; }
    }
}