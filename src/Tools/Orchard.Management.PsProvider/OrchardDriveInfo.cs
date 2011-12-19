using System.Management.Automation;
using Autofac;
using Orchard.Management.PsProvider.Host;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.Management.PsProvider {
    public class OrchardDriveInfo : PSDriveInfo {
        private readonly ILifetimeScope _scope;
        private readonly IPowerShellConsole _console;
        private readonly INavigationProviderManager _navigationProviderManager;

        public OrchardDriveInfo(PSDriveInfo driveInfo, string orchardRoot, ILifetimeScope scope) : base(driveInfo)  {
            OrchardRoot = orchardRoot;
            _scope = scope;
            _console = _scope.Resolve<IPowerShellConsole>();
            _navigationProviderManager = _scope.Resolve<INavigationProviderManager>(
                new NamedParameter("scope", _scope));
        }

        public string OrchardRoot { get; private set; }
        internal IOrchardSession Session { get; private set; }
        internal IOrchardVfs Vfs { get; private set; }

        public void Initialize() {
            IOrchardSession session = _scope.Resolve<IOrchardSession>(
                new NamedParameter("orchardPath", OrchardRoot));
            session.Initialize();
            Session = session;

            InitializeVfs();
        }

        public void Close() {
            if (Session != null) {
                Session.Shutdown();
            }

            _scope.Dispose();
        }

        private void InitializeVfs() {
            IOrchardVfs vfs = _scope.Resolve<IOrchardVfs>(
                new NamedParameter("drive", this));
            vfs.Initialize();
            Vfs = vfs;
        }
    }
}