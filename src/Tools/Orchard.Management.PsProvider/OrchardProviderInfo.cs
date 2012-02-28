// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardProviderInfo.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System.Management.Automation;
    using Autofac;

    /// <summary>
    /// Represents the state of a single instance of the Orchard PS provider.
    /// </summary>
    public class OrchardProviderInfo : ProviderInfo 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderInfo"/> class.
        /// </summary>
        /// <param name="providerInfo">The <see cref="ProviderInfo"/> object of the PS provider.</param>
        /// <param name="container">The dependency injection container of the PS provider.</param>
        internal OrchardProviderInfo(ProviderInfo providerInfo, IContainer container) 
            : base(providerInfo)
        {
            this.Container = container;
        }

        /// <summary>
        /// Gets the dependency injection container of the PS provider.
        /// </summary>
        internal IContainer Container { get; private set; }
    }
}