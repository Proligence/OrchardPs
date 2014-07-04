// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPathValidator.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Navigation
{
    /// <summary>
    /// Defines the API for classes which validate input paths for VFS-based PS providers.
    /// </summary>
    public interface IPathValidator
    {
        /// <summary>
        /// Determines whether the specified path is syntactically valid.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns><c>true</c> if the specified path is valid path; otherwise, <c>false</c>.</returns>
        bool IsValidPath(string path);

        /// <summary>
        /// Determines whether the specified path represents the root of the VFS drive.
        /// </summary>
        /// <param name="path">The path to examine.</param>
        /// <param name="root">The root path of the VFS drive.</param>
        /// <returns><c>true</c> if the path represents the root path; otherwise, <c>false</c>.</returns>
        bool IsDrivePath(string path, string root);

        /// <summary>
        /// Joins two paths.
        /// </summary>
        /// <param name="left">The left part of the path.</param>
        /// <param name="right">The right part of the path.</param>
        /// <returns>The joined path.</returns>
        string JoinPath(string left, string right);

        /// <summary>
        /// Normalizes the specified path.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The specified path in normalized form.</returns>
        string NormalizePath(string path);
    }
}