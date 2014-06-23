// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappableAttribute.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider
{
    using System;

    /// <summary>
    /// Marks properties which should be mapped using <see cref="IPropertyMapper"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MappableAttribute : Attribute
    {
        /// <summary>
        /// Specifies whether the property should be mapped.
        /// </summary>
        private readonly bool mappable;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappableAttribute"/> class.
        /// </summary>
        /// <param name="mappable"><c>true</c> if the property should be mapped; otherwise, <c>false</c>.</param>
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