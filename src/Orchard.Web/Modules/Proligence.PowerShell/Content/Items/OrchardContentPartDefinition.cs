namespace Proligence.PowerShell.Content.Items
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the definition of an Orchard content part.
    /// </summary>
    [Serializable]
    public class OrchardContentPartDefinition
    {
        /// <summary>
        /// Gets or sets the name of the tenant to which the content part definition belongs.
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the name of the content part.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content fields of the content part.
        /// </summary>
        public OrchardContentFieldDefinition[] Fields { get; set; }

        /// <summary>
        /// Gets or sets the settings of the content part.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public IDictionary<string, string> Settings { get; set; }
    }
}