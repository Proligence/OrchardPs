using Orchard.Management.PsProvider.Agents;

namespace Orchard.Management.PsProvider.Host {
    internal class OrchardSession : IOrchardSession {
        private readonly OrchardHostContextProvider _hostContextProvider;
        private OrchardHostContext _context;

        public OrchardSession(string orchardPath) {
            _hostContextProvider = new OrchardHostContextProvider(orchardPath);
        }

        public void Initialize() {
            OrchardHostContext context = _hostContextProvider.CreateContext();
            if (context.StartSessionResult == context.RetryResult) {
                context = _hostContextProvider.CreateContext();
            }
            else if (context.StartSessionResult == ReturnCodes.Fail) {
                _hostContextProvider.Shutdown(_context);
                throw new OrchardProviderException("Failed to initialize Orchard session.");
            }
            
            _context = context;
        }

        public void Shutdown() {
            _hostContextProvider.Shutdown(_context);
        }

        public T CreateAgent<T>() where T : AgentProxy {
            return _hostContextProvider.CreateAgent<T>();
        }
    }
}