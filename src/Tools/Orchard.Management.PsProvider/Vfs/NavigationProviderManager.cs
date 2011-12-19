using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Autofac;

namespace Orchard.Management.PsProvider.Vfs {
    public class NavigationProviderManager : INavigationProviderManager {
        private readonly IPowerShellConsole _console;
        private readonly ILifetimeScope _scope;
        private IPsNavigationProvider[] _navigationProviders;

        public NavigationProviderManager(IPowerShellConsole console, ILifetimeScope scope) {
            _console = console;
            _scope = scope;
        }

        public IEnumerable<IPsNavigationProvider> GetProviders() {
            if (_navigationProviders == null) {
                IEnumerable<Type> types = GetNavigationProviderTypes();

                var navigationProviders = new List<IPsNavigationProvider>();
                foreach (Type providerType in types) {
                    try {
                        IPsNavigationProvider provider = CreateNavigationProvider(providerType);
                        navigationProviders.Add(provider);
                    }
                    catch (Exception ex) {
                        string message = "Failed to create instance of type '" + providerType.FullName + "'. " + ex.CollectMessages();
                        var exception = new OrchardProviderException(message, ex, false, ErrorIds.CreateNavigationProviderFailed);
                        _console.WriteError(exception, exception.ErrorId, ErrorCategory.NotSpecified);
                    }

                }

                _navigationProviders = navigationProviders.ToArray();
            }

            return (IEnumerable<IPsNavigationProvider>)_navigationProviders.Clone();
        }

        public IEnumerable<IPsNavigationProvider> GetProviders(NodeType nodeType) {
            IEnumerable<IPsNavigationProvider> navigationProviders = GetProviders();
            navigationProviders.OrderBy(np => np.GetPathLength());

            return navigationProviders.Where(np => np.NodeType == nodeType).ToArray();
        }

        private IEnumerable<Type> GetNavigationProviderTypes() {
            List<Type> types = new List<Type>();

            Type navigationProviderType = typeof (IPsNavigationProvider);
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
            
            return types;
        }

        private IPsNavigationProvider CreateNavigationProvider(Type providerType) {
            IPsNavigationProvider provider = (IPsNavigationProvider)Activator.CreateInstance(providerType);
            _scope.InjectUnsetProperties(provider);
            provider.Initialize();
            return provider;
        }
    }
}