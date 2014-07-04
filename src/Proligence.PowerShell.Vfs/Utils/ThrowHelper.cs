// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelper.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Utils
{
    using System;

    /// <summary>
    /// Implements helper methods for creating exception objects.
    /// </summary>
    internal static class ThrowHelper 
    {
        /// <summary>
        /// Creates a <see cref="ArgumentException"/> when the specified path does not represent a valid object.
        /// </summary>
        /// <param name="path">The specified path.</param>
        /// <returns>The exception object.</returns>
        public static ArgumentException InvalidPathException(string path) 
        {
            return new ArgumentException("Path must represent a valid object: " + path);
        }
    }
}