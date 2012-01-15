using System.Management.Automation;
using Autofac;
using Orchard.Management.PsProvider.Host;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.Management.PsProvider {
    public class OrchardDriveInfo : PSDriveInfo {
        private readonly IPowerShellConsole _console;
        private readonly INavigationProviderManager _navigationProviderManager;

        public OrchardDriveInfo(PSDriveInfo driveInfo, string orchardRoot, ILifetimeScope scope) : base(driveInfo)  {
            OrchardRoot = orchardRoot;
            LifetimeScope = scope;
            _console = LifetimeScope.Resolve<IPowerShellConsole>();
            _navigationProviderManager = LifetimeScope.Resolve<INavigationProviderManager>(
                new NamedParameter("scope", LifetimeScope));
        }

        public string OrchardRoot { get; private set; }
        internal ILifetimeScope LifetimeScope { get; private set; }
        internal IOrchardSession Session { get; private set; }
        internal IOrchardVfs Vfs { get; private set; }

        public void Initialize() {
            IOrchardSession session = LifetimeScope.Resolve<IOrchardSession>(
                new NamedParameter("orchardPath", OrchardRoot));
            session.Initialize();
            Session = session;

            InitializeVfs();
        }

        public void Close() {
            if (Session != null) {
                Session.Shutdown();
            }

            LifetimeScope.Dispose();
        }

        internal IContainer GetOrchardProviderContainer() {
            return OrchardProviderContainer.GetContainer();
        }

        private void InitializeVfs() {
            IOrchardVfs vfs = LifetimeScope.Resolve<IOrchardVfs>(
                new NamedParameter("drive", this));
            vfs.Initialize();
            Vfs = vfs;
        }
    }
}