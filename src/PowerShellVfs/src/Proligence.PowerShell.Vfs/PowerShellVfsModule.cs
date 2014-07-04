// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerShellVfsModule.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs
{
    using Autofac;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a Autofac registration module for PowerShell VFS library.
    /// </summary>
    public class PowerShellVfsModule : Module
    {
        /// <summary>
        /// Adds registrations to the specified container builder.
        /// </summary>
        /// <param name="builder">The Autofac container builder.</param>
        protected override void Load(ContainerBuilder builder)
        {
            // Global singletons
            builder.RegisterType<PowerShellConsole>().As<IPowerShellConsole>().SingleInstance();

            // Per-drive singletons
            builder.RegisterType<PowerShellVfs>().As<IPowerShellVfs>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultPathValidator>().As<IPathValidator>().InstancePerLifetimeScope();
            builder.RegisterType<NavigationProviderManager>().As<INavigationProviderManager>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}