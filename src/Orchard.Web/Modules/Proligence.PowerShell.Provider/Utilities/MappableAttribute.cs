using System;

namespace Proligence.PowerShell.Provider.Utilities {
    /// <summary>
    /// Marks properties which should be mapped using <see cref="IPropertyMapper"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MappableAttribute : Attribute {
        private readonly bool _mappable;

        public MappableAttribute(bool mappable = true) {
            _mappable = mappable;
        }

        /// <summary>
        /// Gets a value indicating whether the property should be mapped.
        /// </summary>
        public bool Mappable {
            get { return _mappable; }
        }
    }
}