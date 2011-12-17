using System.Collections.Generic;
using Orchard.Management.PsProvider.Host;

namespace Orchard.Management.PsProvider.Agents {
    internal class AgentManager : IAgentManager {
        private readonly IOrchardSession _session;
        private readonly Dictionary<string, AgentProxy> _agents;

        public AgentManager(IOrchardSession session) {
            _session = session;
            _agents = new Dictionary<string, AgentProxy>();
        }

        public TAgent GetAgent<TAgent>() where TAgent : AgentProxy {
            string name = typeof(TAgent).FullName;
            if (name == null) {
                throw new OrchardProviderException("Invalid agent type specified.");
            }
            
            AgentProxy agent;
            if (_agents.TryGetValue(name, out agent)) {
                return (TAgent)agent;
            }

            TAgent newAgent = _session.CreateAgent<TAgent>();
            _agents.Add(name, newAgent);
            return newAgent;
        }
    }
}