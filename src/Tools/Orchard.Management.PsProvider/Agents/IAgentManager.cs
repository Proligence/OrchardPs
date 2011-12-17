namespace Orchard.Management.PsProvider.Agents {
    public interface IAgentManager {
        TAgent GetAgent<TAgent>() where TAgent : AgentProxy;
    }
}