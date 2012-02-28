// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentManager.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Agents 
{
    using System.Collections.Generic;
    using Orchard.Management.PsProvider.Host;

    /// <summary>
    /// Manages agent classes which expose features outside Orchard's web application AppDomain.
    /// </summary>
    internal class AgentManager : IAgentManager 
    {
        /// <summary>
        /// The Orchard session instance.
        /// </summary>
        private readonly IOrchardSession session;

        /// <summary>
        /// Maps agent type names to their proxy instances.
        /// </summary>
        private readonly Dictionary<string, AgentProxy> agents;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentManager"/> class.
        /// </summary>
        /// <param name="session">The Orchard session instance.</param>
        public AgentManager(IOrchardSession session) 
        {
            this.session = session;
            this.agents = new Dictionary<string, AgentProxy>();
        }

        /// <summary>
        /// Gets a proxy to the agent of the specified type.
        /// </summary>
        /// <typeparam name="TAgent">The type of the agent to get.</typeparam>
        /// <returns>The agent proxy instance.</returns>
        public TAgent GetAgent<TAgent>() where TAgent : AgentProxy 
        {
            string name = typeof(TAgent).FullName;
            if (name == null) 
            {
                throw new OrchardProviderException("Invalid agent type specified.");
            }
            
            AgentProxy agent;
            if (this.agents.TryGetValue(name, out agent)) 
            {
                return (TAgent)agent;
            }

            TAgent newAgent = this.session.CreateAgent<TAgent>();
            this.agents.Add(name, newAgent);
            
            return newAgent;
        }
    }
}