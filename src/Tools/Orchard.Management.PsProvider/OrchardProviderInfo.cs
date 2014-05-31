// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardProviderInfo.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;
    using Autofac;
    using Proligence.PowerShell.Vfs;
    using Proligence.PowerShell.Vfs.Provider;

    /// <summary>
    /// Represents the state of a single instance of the Orchard PS provider.
    /// </summary>
    public class OrchardProviderInfo : VfsProviderInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderInfo"/> class.
        /// </summary>
        /// <param name="providerInfo">The <see cref="ProviderInfo"/> object of the PS provider.</param>
        /// <param name="container">The dependency injection container of the PS provider.</param>
        [ExcludeFromCodeCoverage]
        internal OrchardProviderInfo(ProviderInfo providerInfo, IContainer container) 
            : base(providerInfo, container)
        {
        }
    }
}