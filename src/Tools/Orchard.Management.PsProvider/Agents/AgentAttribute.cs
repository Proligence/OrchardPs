// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentAttribute.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Agents 
{
    using System;

    /// <summary>
    /// Specifies the class that implements an agent interfaces.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class AgentAttribute : Attribute
    {
        /// <summary>
        /// The type which implements the agent.
        /// </summary>
        private readonly Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentAttribute"/> class.
        /// </summary>
        /// <param name="type">The type which implements the agent.</param>
        public AgentAttribute(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets the type which implements the agent.
        /// </summary>
        public Type Type
        {
            get { return this.type; }
        }
    }
}