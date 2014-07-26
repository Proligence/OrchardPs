namespace Proligence.PowerShell.Provider
{
    using System;
    using Autofac;

    /// <summary>
    /// Manages dependency injection containers in the Orchard web application.
    /// </summary>
    public interface IContainerManager : IDisposable 
    {
        /// <summary>
        /// Gets the dependency injection container for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <returns>The dependency injection lifetime scope instance.</returns>
        ILifetimeScope GetTenantContainer(string tenantName);
    }
}