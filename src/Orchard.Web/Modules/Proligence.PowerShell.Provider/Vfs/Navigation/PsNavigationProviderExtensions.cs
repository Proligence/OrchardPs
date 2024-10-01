﻿using Orchard.Validation;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// Implements extension methods for the <see cref="IPsNavigationProvider"/> interface.
    /// </summary>
    public static class PsNavigationProviderExtensions {
        /// <summary>
        /// Gets the depth of the path of the navigation provider.
        /// </summary>
        /// <param name="navigationProvider">The navigation provider.</param>
        /// <returns>The path depth of the navigation provider.</returns>
        public static int GetPathLength(this IPsNavigationProvider navigationProvider) {
            Argument.ThrowIfNull(navigationProvider, "navigationProvider");

            if (!string.IsNullOrEmpty(navigationProvider.Path)) {
                return navigationProvider.Path.Split('\\').Length;
            }

            return 0;
        }
    }
}