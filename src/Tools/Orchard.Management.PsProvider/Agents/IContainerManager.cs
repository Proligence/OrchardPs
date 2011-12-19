using System;
using Autofac;

namespace Orchard.Management.PsProvider.Agents {
    public interface IContainerManager : IDisposable {
        ILifetimeScope GetSiteContainer(string siteName);
    }
}