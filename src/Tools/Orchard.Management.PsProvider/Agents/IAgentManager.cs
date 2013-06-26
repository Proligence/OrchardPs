// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAgentManager.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Agents 
{
    /// <summary>
    /// Manages agent classes which expose features outside Orchard's web application AppDomain.
    /// </summary>
    public interface IAgentManager 
    {
        /// <summary>
        /// Gets a proxy to the agent of the specified type.
        /// </summary>
        /// <typeparam name="TAgent">The type of the agent to get.</typeparam>
        /// <returns>The agent proxy instance.</returns>
        TAgent GetAgent<TAgent>() where TAgent : IAgent;
    }
}