namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Orchard;
    using Orchard.Environment;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.ShellBuilders;

    /// <summary>
    /// Manages dependency injection containers in the Orchard web application.
    /// </summary>
    public class TenantContextManager : ITenantContextManager 
    {
        private readonly IOrchardHost orchardHost;
        private readonly IShellSettingsManager shellSettingsManager;

        public TenantContextManager(IOrchardHost orchardHost, IShellSettingsManager shellSettingsManager)
        {
            this.shellSettingsManager = shellSettingsManager;
            this.orchardHost = orchardHost;
        }

        /// <summary>
        /// Gets the dependency injection container for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <returns>The dependency injection lifetime scope instance.</returns>
        public ILifetimeScope GetTenantContainer(string tenantName) 
        {
            return this.GetShellContext(tenantName).LifetimeScope;
        }

        /// <summary>
        /// Creates a new work context scope for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <returns>The created work context scope.</returns>
        public IWorkContextScope CreateWorkContextScope(string tenantName)
        {
            return this.GetTenantContainer(tenantName).CreateWorkContextScope();
        }

        /// <summary>
        /// Creates a <see cref="ShellContext"/> object for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <returns>The created <see cref="ShellContext"/> object.</returns>
        private ShellContext GetShellContext(string tenantName) 
        {
            IEnumerable<ShellSettings> shellSettings = this.shellSettingsManager.LoadSettings();
            ShellSettings settings = shellSettings.FirstOrDefault(s => s.Name == tenantName);
            if (settings == null) 
            {
                throw new ArgumentException("Failed to find tenant '" + tenantName + "'.", "tenantName");
            }

            return this.orchardHost.GetShellContext(settings);
        }
    }
}