// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VfsDriveInfo.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Provider
{
    using System.IO;
    using System.Management.Automation;
    using Autofac;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Represents the state of a single drive which uses a PS provider based on PowerShell VFS.
    /// </summary>
    public class VfsDriveInfo : PSDriveInfo 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VfsDriveInfo"/> class.
        /// </summary>
        /// <param name="driveInfo">The <see cref="DriveInfo"/> object for the VFS drive.</param>
        /// <param name="scope">The drive's dependency injection lifetime scope.</param>
        public VfsDriveInfo(PSDriveInfo driveInfo, ILifetimeScope scope)
            : base(driveInfo)
        {
            this.LifetimeScope = scope;
            this.Console = this.LifetimeScope.Resolve<IPowerShellConsole>();
            this.NavigationProviderManager = this.LifetimeScope.Resolve<INavigationProviderManager>(
                new NamedParameter("scope", this.LifetimeScope));
        }

        /// <summary>
        /// Gets the drive's dependency injection lifetime scope.
        /// </summary>
        public ILifetimeScope LifetimeScope { get; private set; }

        /// <summary>
        /// Gets the drive's PowerShell virtual file system (VFS) instance.
        /// </summary>
        public IPowerShellVfs Vfs { get; private set; }

        /// <summary>
        /// Gets the object which exposes the PowerShell console.
        /// </summary>
        public IPowerShellConsole Console { get; private set; }

        /// <summary>
        /// Gets the object which exposes the navigation providers for the drive.
        /// </summary>
        public INavigationProviderManager NavigationProviderManager { get; private set; }

        /// <summary>
        /// Initializes the drive.
        /// </summary>
        public virtual void Initialize()
        {
            this.InitializeVfs();
        }

        /// <summary>
        /// Closes the drive and disposes of all used resources.
        /// </summary>
        public virtual void Close()
        {
            this.LifetimeScope.Dispose();
        }

        /// <summary>
        /// Initializes the drive's PowerShell virtual file system (VFS).
        /// </summary>
        private void InitializeVfs() 
        {
            IPowerShellVfs vfs = this.LifetimeScope.Resolve<IPowerShellVfs>(
                new NamedParameter("drive", this));
            vfs.Initialize();
            this.Vfs = vfs;
        }
    }
}