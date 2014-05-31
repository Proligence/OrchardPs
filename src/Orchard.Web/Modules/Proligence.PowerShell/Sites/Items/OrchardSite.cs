// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardSite.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Sites.Items 
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Orchard.Environment.Configuration;

    /// <summary>
    /// Implements an object which represents an Orchard site.
    /// </summary>
    [Serializable]
    public class OrchardSite 
    {
        /// <summary>
        /// Gets or sets the name of the site.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current state of the site.
        /// </summary>
        public TenantState State { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the site's database.
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the data provider used by the site.
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// Gets or sets the site's data table prefix.
        /// </summary>
        public string DataTablePrefix { get; set; }

        /// <summary>
        /// Gets or sets the name of the site's encryption algorithm.
        /// </summary>
        public string EncryptionAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the site's encryption key.
        /// </summary>
        public string EncryptionKey { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the site's hash algorithm.
        /// </summary>
        public string HashAlgorithm { get; set; }
        
        /// <summary>
        /// Gets or sets the site's hash key.
        /// </summary>
        public string HashKey { get; set; }

        /// <summary>
        /// Gets or sets the site's request URL host.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design")]
        public string RequestUrlHost { get; set; }
        
        /// <summary>
        /// Gets or sets the site's request URL prefix.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design")]
        public string RequestUrlPrefix { get; set; }
    }
}