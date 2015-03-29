namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting.Lifetime;
    using System.Security;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Autofac;
    using Orchard.Environment;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.ShellBuilders;

    public class PsProviderAgent : MarshalByRefObject, IDisposable, IRegisteredObject
    {
        private readonly IContainer hostContainer;
        private ShellContext defaultShellContext;

        public PsProviderAgent()
        {
            this.hostContainer = OrchardStarter.CreateHostContainer(ContainerRegistrations);
        }

        /// <summary>
        /// Gets the PS session manager for the default tenant.
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
        /// <returns>
        /// An object of type <see cref="ILease"/> used to control the lifetime policy for this instance. This is the
        /// current lifetime service object for this instance if one exists; otherwise, a new lifetime service object
        /// initialized to the value of the <see cref="LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            // never expire the license
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