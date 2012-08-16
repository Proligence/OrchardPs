// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelper.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider
{
    using System;

    /// <summary>
    /// Implements helper methods for creating exception objects.
    /// </summary>
    internal static class ThrowHelper 
    {
        /// <summary>
        /// Creates a <see cref="ArgumentException"/> when the specified path does not contain an Orchard installation.
        /// </summary>
        /// <param name="rootPath">The specified path.</param>
        /// <returns>The exception object.</returns>
        public static ArgumentException InvalidRootPathException(string rootPath)
        {
            return new ArgumentException("The directory '" + rootPath + "' does not contain an Orchard installation.");
        }

        /// <summary>
        /// Creates a <see cref="ArgumentException"/> when the specified path does not represent a valid Orchard
        /// object.
        /// </summary>
        /// <param name="path">The specified path.</param>
        /// <returns>The exception object.</returns>
        public static ArgumentException InvalidPathException(string path) 
        {
            return new ArgumentException("Path must represent a valid Orchard object: " + path);
        }

        /// <summary>
        /// Creates a <see cref="InvalidOperationException"/> when an operation which requires an Orchard drive is
        /// invoked from a path belonging to another (non-Orchard) PS provider.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <returns>The exception object.</returns>
        public static InvalidOperationException InvalidOperation(string message) 
        {
            return new InvalidOperationException(message);
        }
    }
}