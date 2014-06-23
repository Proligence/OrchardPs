// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentBase.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Agents 
{
    using System;
    using System.Runtime.Remoting.Lifetime;
    using System.Security;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Autofac;
    using Orchard.Environment;

    /// <summary>
    /// The base class for classes which implement agents which expose features outside Orchard's web application
    /// AppDomain.
    /// </summary>
    public class AgentBase : MarshalByRefObject, IAgent, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentBase"/> class.
        /// </summary>
        public AgentBase()
        {
            this.HostContainer = OrchardStarter.CreateHostContainer(this.ContainerRegistrations);
            this.ContainerManager = this.HostContainer.Resolve<IContainerManager>();
        }

        /// <summary>
        /// Gets the dependency injection container of the Orchard web application.
        /// </summary>
        protected IContainer HostContainer { get; private set; }

        /// <summary>
        /// Gets object which manages dependency injection containers in the Orchard web application.
        /// </summary>
        protected IContainerManager ContainerManager { get; private set; }

        /// <summary>
        /// Releases the agent and all resources used by it.
        /// </summary>
        public void Dispose() 
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        /// <summary>
        /// Registers types in the <see cref="HostContainer"/> dependency injection container.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        protected void ContainerRegistrations(ContainerBuilder builder) 
        {
            // MVC Singletons
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();

            builder.RegisterType<ContainerManager>().As<IContainerManager>().SingleInstance();
        }

        /// <summary>
        /// Gets the <see cref="T"/> instance for the specified tenant.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>The <see cref="T"/> instance for the specified tenant.</returns>
        protected T Resolve<T>(string tenant)
        {
            ILifetimeScope tenantContainer = this.ContainerManager.GetTenantContainer(tenant);
            return tenantContainer.Resolve<T>();
        }

        /// <summary>
        /// Creates a work context scope for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>The created <see cref="IWorkContextScope"/>.</returns>
        protected IWorkContextScope CreateWorkContextScope(string tenant)
        {
            ILifetimeScope tenantContainer = this.ContainerManager.GetTenantContainer(tenant);
            return tenantContainer.CreateWorkContextScope();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.HostContainer != null)
                {
                    this.HostContainer.Dispose();
                }

                if (this.ContainerManager != null)
                {
                    this.ContainerManager.Dispose();
                }
            }
        }
    }
}