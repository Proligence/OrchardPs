// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PsNavigationProviderExtensions.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    using System;

    /// <summary>
    /// Implements extension methods for the <see cref="IPsNavigationProvider"/> interface.
    /// </summary>
    public static class PsNavigationProviderExtensions 
    {
        /// <summary>
        /// Gets the depth of the path of the navigation provider.
        /// </summary>
        /// <param name="navigationProvider">The navigation provider.</param>
        /// <returns>The path depth of the navigation provider.</returns>
        public static int GetPathLength(this IPsNavigationProvider navigationProvider) 
        {
            if (navigationProvider == null)
            {
                throw new ArgumentNullException("navigationProvider");
            }

            if (!string.IsNullOrEmpty(navigationProvider.Path)) 
            {
                return navigationProvider.Path.Split('\\').Length;
            }

            return 0;
        }
    }
}