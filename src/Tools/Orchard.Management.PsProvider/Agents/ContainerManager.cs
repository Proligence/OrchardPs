using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders;

namespace Orchard.Management.PsProvider.Agents {
    public class ContainerManager : IContainerManager {
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly Dictionary<string, ShellContext> _siteShells;

        public ContainerManager(IShellSettingsManager shellSettingsManager, IShellContextFactory shellContextFactory) {
            _shellSettingsManager = shellSettingsManager;
            _shellContextFactory = shellContextFactory;
            _siteShells = new Dictionary<string, ShellContext>();
        }

        public ILifetimeScope GetSiteContainer(string siteName) {
            ShellContext shellContext;
            lock (_siteShells) {
                if (!_siteShells.TryGetValue(siteName, out shellContext)) {
                    shellContext = CreateShellContext(siteName);
                    _siteShells.Add(siteName, shellContext);
                }
            }
            
            return shellContext.LifetimeScope;
        }

        private ShellContext CreateShellContext(string siteName) {
            IEnumerable<ShellSettings> shellSettings = _shellSettingsManager.LoadSettings();
            ShellSettings settings = shellSettings.Where(s => s.Name == siteName).FirstOrDefault();
            if (settings == null) {
                throw new ArgumentException("Failed to find site '" + siteName + "'.", "siteName");
            }

            return _shellContextFactory.CreateShellContext(settings);
        }

        public void Dispose() {
            lock (_siteShells) {
                foreach (ShellContext shellContext in _siteShells.Values) {
                    shellContext.LifetimeScope.Dispose();
                }
                
                _siteShells.Clear();
            }
        }
    }
}