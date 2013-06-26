// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrchardSession.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Host 
{
    using System;
    using Orchard.Management.PsProvider.Agents;

    /// <summary>
    /// Represents the connection with the AppDomain of the Orchard web application.
    /// </summary>
    public interface IOrchardSession 
    {
        /// <summary>
        /// Initializes the Orchard session.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shuts down the Orchard session.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Creates a proxy for an agent of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the agent to create.</typeparam>
        /// <param name="agentType">The type of the agent to create.</param>
        /// <returns>The created agent proxy instance.</returns>
        T CreateAgent<T>(Type agentType) where T : IAgent;
    }
}