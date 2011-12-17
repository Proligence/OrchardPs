using Orchard.Management.PsProvider.Agents;

namespace Orchard.Management.PsProvider.Host {
    public interface IOrchardSession {
        void Initialize();
        void Shutdown();
        T CreateAgent<T>() where T : AgentProxy;
    }
}