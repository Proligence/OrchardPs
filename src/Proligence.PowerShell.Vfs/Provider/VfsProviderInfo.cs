// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VfsProviderInfo.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Provider
{
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;
    using Autofac;

    /// <summary>
    /// Represents the state of a single instance of the VFS-based PS provider.
    /// </summary>
    public class VfsProviderInfo : ProviderInfo 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VfsProviderInfo"/> class.
        /// </summary>
        /// <param name="providerInfo">The <see cref="ProviderInfo"/> object of the PS provider.</param>
        /// <param name="container">The dependency injection container of the PS provider.</param>
        [ExcludeFromCodeCoverage]
        public VfsProviderInfo(ProviderInfo providerInfo, IContainer container) 
            : base(providerInfo)
        {
            this.Container = container;
        }

        /// <summary>
        /// Gets the dependency injection container of the PS provider.
        /// </summary>
        public IContainer Container { get; private set; }
    }
}