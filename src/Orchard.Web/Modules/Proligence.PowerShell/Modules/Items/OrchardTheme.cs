// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardTheme.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Items
{
    using System;
    
    /// <summary>
    /// Represents a theme.
    /// </summary>
    [Serializable]
    public class OrchardTheme
    {
        /// <summary>
        /// Gets or sets the Orchard module which provides the theme.
        /// </summary>
        public OrchardModule Module { get; set; }

        /// <summary>
        /// Gets the theme's name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Module.Name;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the theme is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the theme needs a data update / migration.
        /// </summary>
        public bool NeedsUpdate { get; set; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}