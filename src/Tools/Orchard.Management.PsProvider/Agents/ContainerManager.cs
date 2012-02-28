// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerManager.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
        private readonly Dictionary<string, ShellContext> siteShells;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerManager"/> class.
        /// </summary>
        /// <param name="shellSettingsManager">The shell settings manager.</param>
        /// <param name="shellContextFactory">The shell context factory.</param>
        public ContainerManager(IShellSettingsManager shellSettingsManager, IShellContextFactory shellContextFactory)
        {
            this.shellSettingsManager = shellSettingsManager;
            this.shellContextFactory = shellContextFactory;
            this.siteShells = new Dictionary<string, ShellContext>();
        }

        /// <summary>
        /// Gets the dependency injection container for the specified site (tenant).
        /// </summary>
        /// <param name="siteName">The name of the site.</param>
        /// <returns>The dependency injection lifetime scope instance.</returns>
        public ILifetimeScope GetSiteContainer(string siteName) 
        {
            ShellContext shellContext;
            lock (this.siteShells) 
            {
                if (!this.siteShells.TryGetValue(siteName, out shellContext)) 
                {
                    shellContext = this.CreateShellContext(siteName);
                    this.siteShells.Add(siteName, shellContext);
                }
            }
            
            return shellContext.LifetimeScope;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() 
        {
            lock (this.siteShells) 
            {
                foreach (ShellContext shellContext in this.siteShells.Values) 
                {
                    shellContext.LifetimeScope.Dispose();
                }
                
                this.siteShells.Clear();
            }
        }

        /// <summary>
        /// Creates a <see cref="ShellContext"/> object for the specified site.
        /// </summary>
        /// <param name="siteName">The name of the site.</param>
        /// <returns>The created <see cref="ShellContext"/> object.</returns>
        private ShellContext CreateShellContext(string siteName) 
        {
            IEnumerable<ShellSettings> shellSettings = this.shellSettingsManager.LoadSettings();
            ShellSettings settings = shellSettings.Where(s => s.Name == siteName).FirstOrDefault();
            if (settings == null) 
            {
                throw new ArgumentException("Failed to find site '" + siteName + "'.", "siteName");
            }

            return this.shellContextFactory.CreateShellContext(settings);
        }
    }
}