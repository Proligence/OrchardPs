// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentBase.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Agents 
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Autofac;
    using Orchard.Environment;

    /// <summary>
    /// The base class for classes which implement agents which expose features outside Orchard's web application
    /// AppDomain.
    /// </summary>
    public class AgentBase 
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
            if (this.HostContainer != null) 
            {
                this.HostContainer.Dispose();
            }

            if (this.ContainerManager != null) 
            {
                this.ContainerManager.Dispose();
            }
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
    }
}