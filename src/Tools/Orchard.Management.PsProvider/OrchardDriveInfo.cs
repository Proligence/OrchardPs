using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using Autofac;
using Orchard.Management.PsProvider.Host;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.Management.PsProvider {
    public class OrchardDriveInfo : PSDriveInfo {
        private readonly ILifetimeScope _scope;
        private readonly IPowerShellConsole _console;

        public OrchardDriveInfo(PSDriveInfo driveInfo, string orchardRoot, ILifetimeScope scope) : base(driveInfo)  {
            OrchardRoot = orchardRoot;
            _scope = scope;
            _console = _scope.Resolve<IPowerShellConsole>();
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
            IEnumerable<IPsNavigationProvider> navigationProviders = GetNavigationProviders();
            IOrchardVfs vfs = _scope.Resolve<IOrchardVfs>(
                new NamedParameter("drive", this));
            vfs.Initialize(navigationProviders);
            Vfs = vfs;
        }

        private IEnumerable<IPsNavigationProvider> GetNavigationProviders() {
            List<Type> types = new List<Type>();

            Type navigationProviderType = typeof(IPsNavigationProvider);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                try {
                    Type[] assemblyTypes = assembly.GetTypes();
                    foreach (Type type in assemblyTypes) {
                        try {
                            if (type.IsInterface || type.IsAbstract) {
                                continue;
                            }
                            if (navigationProviderType.IsAssignableFrom(type)) {
                                types.Add(type);
                            }
                        }
                        catch (Exception ex) {
                            _console.WriteWarning("Failed to get information about type '" + type.FullName + "'. " + ex.Message);
                        }
                    }
                }
                catch (Exception ex) {
                    _console.WriteWarning("Failed to get types for assembly '" + assembly.FullName + "'. " + ex.Message);
                }
            }
            
            var navigationProviders = new List<IPsNavigationProvider>();
            foreach (Type providerType in types) {
                try {
                    IPsNavigationProvider provider = (IPsNavigationProvider)Activator.CreateInstance(providerType);
                    _scope.InjectUnsetProperties(provider);
                    provider.Initialize();

                    navigationProviders.Add(provider);
                }
                catch (Exception ex) {
                    string message = "Failed to create instance of type '" + providerType.FullName + "'. " + ex.CollectMessages();
                    var exception = new OrchardProviderException(message, ex, false, ErrorIds.CreateNavigationProviderFailed);
                    _console.WriteError(exception, exception.ErrorId, ErrorCategory.NotSpecified);
                }

            }

            return navigationProviders;
        }
    }
}