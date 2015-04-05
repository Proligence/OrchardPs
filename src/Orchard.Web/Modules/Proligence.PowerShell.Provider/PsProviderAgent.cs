namespace Proligence.PowerShell.Provider
{
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

    /// <summary>
    /// Implements an agent for accessing the Orchard PowerShell Engine using remoting.
    /// </summary>
    /// <remarks>
    /// This class can be instantiated using remoting, in a separate <c>AppDomain</c> which hosts the Orchard
    /// framework. This is how the <c>OrchardPs.exe</c> utility works. Additionally, this class is used in a
    /// simliary way by the OrchardPs unit testing infrastructure.
    /// </remarks>
    public class PsProviderAgent : MarshalByRefObject, IDisposable, IRegisteredObject
    {
        private readonly IContainer hostContainer;
        private ShellContext defaultShellContext;

        public PsProviderAgent()
        {
            this.hostContainer = OrchardStarter.CreateHostContainer(ContainerRegistrations);
        }

        /// <summary>
        /// Gets the PowerShell session manager for the default tenant.
        /// </summary>
        public IPsSessionManager GetSessionManager()
        {
            if (this.defaultShellContext == null)
            {
                this.defaultShellContext = this.CreateDefaultShellContext();
            }

            return this.defaultShellContext.LifetimeScope.Resolve<IPsSessionManager>();
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
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
        public void Stop(bool immediate)
        {
            HostingEnvironment.UnregisterObject(this);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.defaultShellContext != null)
                {
                    this.defaultShellContext.Dispose();
                }

                if (this.hostContainer != null)
                {
                    this.hostContainer.Dispose();
                }
            }
        }

        private static void ContainerRegistrations(ContainerBuilder builder)
        {
            // MVC Singletons
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();
        }

        private ShellContext CreateDefaultShellContext()
        {
            var shellSettings = this.hostContainer.Resolve<IShellSettingsManager>().LoadSettings();
            var settings = shellSettings.FirstOrDefault(s => s.Name == "Default");
            var shellContextFactory = this.hostContainer.Resolve<IShellContextFactory>();
            return shellContextFactory.CreateShellContext(settings);
        }
    }
}