// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CmdletAliasAttribute.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System;

    /// <summary>
    /// Specifies an alias for a cmdlet which will be automatically registered during application startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class CmdletAliasAttribute : Attribute 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CmdletAliasAttribute"/> class.
        /// </summary>
        /// <param name="alias">The alias to register.</param>
        public CmdletAliasAttribute(string alias) 
        {
            if (string.IsNullOrEmpty(alias)) 
            {
                throw new ArgumentNullException("alias");
            }
            
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the command's alias.
        /// </summary>
        public string Alias { get; private set; }
    }
}