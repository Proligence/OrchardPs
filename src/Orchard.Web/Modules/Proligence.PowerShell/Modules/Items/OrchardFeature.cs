// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardFeature.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Items
{
    using System;

    /// <summary>
    /// Represents an Orchard feature.
    /// </summary>
    [Serializable]
    public class OrchardFeature
    {
        /// <summary>
        /// Gets or sets the identifier of the feature.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the feature.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the feature's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the feature's category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the feature's priority.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the names of the dependant features for the feature.
        /// </summary>
        public string[] Dependencies { get; set; }
        
        /// <summary>
        /// Gets or sets the Orchard module which implements the feature.
        /// </summary>
        public OrchardModule Module { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the feature is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the Orchard tenant which the feature belongs to.
        /// </summary>
        public string TenantName { get; set; }
    }
}