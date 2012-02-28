// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentAttribute.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Agents 
{
    using System;

    /// <summary>
    /// Specifies that a class implements a proxy to an agent which exposes features outside Orchard's web application
    /// AppDomain.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AgentAttribute : Attribute 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentAttribute"/> class.
        /// </summary>
        /// <param name="typeName">The full name of the type which implements the agent.</param>
        public AgentAttribute(string typeName) 
        {
            this.TypeName = typeName;
        }

        /// <summary>
        /// Gets the full name of the type which implements the agent.
        /// </summary>
        public string TypeName { get; private set; }
    }
}