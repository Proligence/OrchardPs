namespace Proligence.PowerShell.Provider.Utilities
{
    using System;

    /// <summary>
    /// Marks properties which should be mapped using <see cref="IPropertyMapper"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MappableAttribute : Attribute
    {
        private readonly bool mappable;

        public MappableAttribute(bool mappable = true)
        {
            this.mappable = mappable;
        }

        /// <summary>
        /// Gets a value indicating whether the property should be mapped.
        /// </summary>
        public bool Mappable
        {
            get
            {
                return this.mappable;
            }
        }
    }
}