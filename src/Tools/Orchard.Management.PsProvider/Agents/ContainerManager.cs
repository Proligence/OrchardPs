namespace Orchard.Management.PsProvider.Agents 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Orchard.Environment.ShellBuilders;

    /// <summary>
    /// Manages dependency injection containers in the Orchard web application.
    /// </summary>
    public class ContainerManager : IContainerManager 
    {
        private readonly IShellSettingsManager shellSettingsManager;
        private readonly IShellContextFactory shellContextFactory;
        private readonly Dictionary<string, ShellContext> tenantShells;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerManager"/> class.
        /// </summary>
        /// <param name="shellSettingsManager">The shell settings manager.</param>
        /// <param name="shellContextFactory">The shell context factory.</param>
        public ContainerManager(IShellSettingsManager shellSettingsManager, IShellContextFactory shellContextFactory)
        {
            this.shellSettingsManager = shellSettingsManager;
            this.shellContextFactory = shellContextFactory;
            this.tenantShells = new Dictionary<string, ShellContext>();
        }

        /// <summary>
        /// Gets the dependency injection container for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <returns>The dependency injection lifetime scope instance.</returns>
        public ILifetimeScope GetTenantContainer(string tenantName) 
        {
            ShellContext shellContext;
            lock (this.tenantShells) 
            {
                if (!this.tenantShells.TryGetValue(tenantName, out shellContext)) 
                {
                    shellContext = this.CreateShellContext(tenantName);
                    this.tenantShells.Add(tenantName, shellContext);
                }
            }
            
            return shellContext.LifetimeScope;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() 
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this.tenantShells)
                {
                    foreach (ShellContext shellContext in this.tenantShells.Values)
                    {
                        shellContext.LifetimeScope.Dispose();
                    }

                    this.tenantShells.Clear();
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="ShellContext"/> object for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <returns>The created <see cref="ShellContext"/> object.</returns>
        private ShellContext CreateShellContext(string tenantName) 
        {
            IEnumerable<ShellSettings> shellSettings = this.shellSettingsManager.LoadSettings();
            ShellSettings settings = shellSettings.FirstOrDefault(s => s.Name == tenantName);
            if (settings == null) 
            {
                throw new ArgumentException("Failed to find tenant '" + tenantName + "'.", "tenantName");
            }

            return this.shellContextFactory.CreateShellContext(settings);
        }
    }
}