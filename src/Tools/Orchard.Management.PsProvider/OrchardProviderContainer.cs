using System;
using Autofac;
using Orchard.Management.PsProvider.Agents;
using Orchard.Management.PsProvider.Host;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.Management.PsProvider {
    public static class OrchardProviderContainer {
        private static IContainer _container;
        private static readonly object _containerLock = new object();

        public static IContainer GetContainer() {
            if (_container == null) {
                lock (_containerLock) {
                    if (_container == null) {
                        _container = BuildContainer();
                    }
                }
            }

            return _container;
        }

        public static IContainer BuildContainer(Action<ContainerBuilder> registrations = null) {
            var builder = new ContainerBuilder();

            builder.RegisterType<PowerShellConsole>().As<IPowerShellConsole>().SingleInstance();

            builder.RegisterType<OrchardDriveInfo>().InstancePerLifetimeScope();
            builder.RegisterType<OrchardSession>().As<IOrchardSession>().InstancePerLifetimeScope();
            builder.RegisterType<OrchardVfs>().As<IOrchardVfs>().InstancePerLifetimeScope();
            builder.RegisterType<AgentManager>().As<IAgentManager>().InstancePerLifetimeScope();

            if (registrations != null) {
                registrations(builder);
            }

            return builder.Build();
        }
    }
}