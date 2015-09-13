using System;
using System.Linq;
using System.Security;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.ShellBuilders;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// Implements an agent for accessing the Orchard PowerShell Engine using remoting.
    /// </summary>
    /// <remarks>
    /// This class can be instantiated using remoting, in a separate <c>AppDomain</c> which hosts the Orchard
    /// framework. This is how the <c>OrchardPs.exe</c> utility works. Additionally, this class is used in a
    /// simliary way by the OrchardPs unit testing infrastructure.
    /// </remarks>
    public class PsProviderAgent : MarshalByRefObject, IDisposable, IRegisteredObject {
        private readonly IContainer _hostContainer;
        private ShellContext _defaultShellContext;

        public PsProviderAgent() {
            _hostContainer = OrchardStarter.CreateHostContainer(ContainerRegistrations);
        }

        /// <summary>
        /// Gets the PowerShell session manager for the default tenant.
        /// </summary>
        public IPsSessionManager GetSessionManager() {
            if (_defaultShellContext == null) {
                _defaultShellContext = CreateDefaultShellContext();
            }

            return _defaultShellContext.LifetimeScope.Resolve<IPsSessionManager>();
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        [SecurityCritical]
        public override object InitializeLifetimeService() {
            return null;
        }

        /// <summary>
        /// Requests a registered object to unregister.
        /// </summary>
        /// <param name="immediate">
        /// <c>true</c> to indicate the registered object should unregister from the hosting environment before
        /// returning; otherwise, <c>false</c>.
        /// </param>
        [SecuritySafeCritical]
        public void Stop(bool immediate) {
            HostingEnvironment.UnregisterObject(this);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_defaultShellContext != null) {
                    _defaultShellContext.Dispose();
                }

                if (_hostContainer != null) {
                    _hostContainer.Dispose();
                }
            }
        }

        private static void ContainerRegistrations(ContainerBuilder builder) {
            // MVC Singletons
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();
        }

        private ShellContext CreateDefaultShellContext() {
            var shellSettings = _hostContainer.Resolve<IShellSettingsManager>().LoadSettings();
            var settings = shellSettings.FirstOrDefault(s => s.Name == "Default");
            var shellContextFactory = _hostContainer.Resolve<IShellContextFactory>();
            return shellContextFactory.CreateShellContext(settings);
        }
    }
}