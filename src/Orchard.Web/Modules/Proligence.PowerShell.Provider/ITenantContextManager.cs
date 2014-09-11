namespace Proligence.PowerShell.Provider
{
    using Autofac;
    using Orchard;

    /// <summary>
    /// Manages dependency injection containers in the Orchard web application.
    /// </summary>
    public interface ITenantContextManager : ISingletonDependency
    {
        /// <summary>
        /// Gets the dependency injection container for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <returns>The dependency injection lifetime scope instance.</returns>
        ILifetimeScope GetTenantContainer(string tenantName);

        /// <summary>
        /// Creates a new work context scope for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <returns>The created work context scope.</returns>
        IWorkContextScope CreateWorkContextScope(string tenantName);
    }
}