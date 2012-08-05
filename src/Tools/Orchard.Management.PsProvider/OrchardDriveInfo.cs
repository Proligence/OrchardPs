// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardDriveInfo.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System.IO;
    using System.Management.Automation;
    using Autofac;
    using Orchard.Management.PsProvider.Host;
    using Orchard.Management.PsProvider.Vfs;

    /// <summary>
    /// Represents the state of a single Orchard drive in the Orchard PS provider.
    /// </summary>
    public class OrchardDriveInfo : PSDriveInfo 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardDriveInfo"/> class.
        /// </summary>
        /// <param name="driveInfo">The <see cref="DriveInfo"/> object for the Orchard drive.</param>
        /// <param name="orchardRoot">The root directory of the Orchard installation represented by the drive.</param>
        /// <param name="scope">The drive's dependency injection lifetime scope.</param>
        public OrchardDriveInfo(PSDriveInfo driveInfo, string orchardRoot, ILifetimeScope scope) : base(driveInfo)
        {
            this.OrchardRoot = orchardRoot;
            this.LifetimeScope = scope;
            this.Console = this.LifetimeScope.Resolve<IPowerShellConsole>();
            this.NavigationProviderManager = this.LifetimeScope.Resolve<INavigationProviderManager>(
                new NamedParameter("scope", this.LifetimeScope));
        }

        /// <summary>
        /// Gets the root directory of the Orchard installation represented by the drive.
        /// </summary>
        public string OrchardRoot { get; private set; }

        /// <summary>
        /// Gets the drive's dependency injection lifetime scope.
        /// </summary>
        internal ILifetimeScope LifetimeScope { get; private set; }

        /// <summary>
        /// Gets the <see cref="IOrchardSession"/> object associated with the Orchard drive.
        /// </summary>
        internal IOrchardSession Session { get; private set; }

        /// <summary>
        /// Gets the drive's Orchard virtual file system (VFS).
        /// </summary>
        internal IOrchardVfs Vfs { get; private set; }

        /// <summary>
        /// Gets the object which exposes the PowerShell console.
        /// </summary>
        internal IPowerShellConsole Console { get; private set; }

        /// <summary>
        /// Gets the object which exposes the navigation providers for the drive.
        /// </summary>
        internal INavigationProviderManager NavigationProviderManager { get; private set; }

        /// <summary>
        /// Initializes the drive and establishes a connection with the AppDomain which runs inside the Orchard web
        /// application.
        /// </summary>
        public void Initialize()
        {
            IOrchardSession session = this.LifetimeScope.Resolve<IOrchardSession>(
                new NamedParameter("orchardPath", this.OrchardRoot));
            session.Initialize();
            this.Session = session;

            this.InitializeVfs();
        }

        /// <summary>
        /// Closes the drive and shutdowns the connection with the AppDomain which runs inside the Orchard web
        /// application.
        /// </summary>
        public void Close()
        {
            if (this.Session != null)
            {
                this.Session.Shutdown();
            }

            this.LifetimeScope.Dispose();
        }

        /// <summary>
        /// Gets the dependency injection container for the Orchard PS provider.
        /// </summary>
        /// <returns>The dependency injection container instance.</returns>
        internal IContainer GetOrchardProviderContainer() 
        {
            return OrchardProviderContainer.GetContainer();
        }

        /// <summary>
        /// Initializes the drive's Orchard virtual file system (VFS).
        /// </summary>
        private void InitializeVfs() 
        {
            IOrchardVfs vfs = this.LifetimeScope.Resolve<IOrchardVfs>(
                new NamedParameter("drive", this));
            vfs.Initialize();
            this.Vfs = vfs;
        }
    }
}