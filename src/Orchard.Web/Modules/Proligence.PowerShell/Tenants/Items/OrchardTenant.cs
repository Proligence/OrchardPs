namespace Proligence.PowerShell.Tenants.Items 
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Orchard.Environment.Configuration;

    /// <summary>
    /// Implements an object which represents an Orchard tenant.
    /// </summary>
    [Serializable]
    public class OrchardTenant 
    {
        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the current state of the tenant.
        /// </summary>
        public TenantState State { get; set; }

        /// <summary>
        /// Gets or sets the connection string to the tenant's database.
        /// </summary>
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the data provider used by the tenant.
        /// </summary>
        public string DataProvider { get; set; }

        /// <summary>
        /// Gets or sets the tenant's data table prefix.
        /// </summary>
        public string DataTablePrefix { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant's encryption algorithm.
        /// </summary>
        public string EncryptionAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the tenant's encryption key.
        /// </summary>
        public string EncryptionKey { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the tenant's hash algorithm.
        /// </summary>
        public string HashAlgorithm { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant's hash key.
        /// </summary>
        public string HashKey { get; set; }

        /// <summary>
        /// Gets or sets the tenant's request URL host.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design")]
        public string RequestUrlHost { get; set; }
        
        /// <summary>
        /// Gets or sets the tenant's request URL prefix.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design")]
        public string RequestUrlPrefix { get; set; }

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