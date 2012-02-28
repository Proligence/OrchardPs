// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardPath.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System.Linq;

    /// <summary>
    /// Implements methods which manipulate the paths which are used to address objects in the Orchard virtual file
    /// system (VFS).
    /// </summary>
    internal static class OrchardPath
    {
        /// <summary>
        /// The character which is used to separate path chunks.
        /// </summary>
        public const string PathSeparator = "\\";

        /// <summary>
        /// Determines whether the specified path is syntatically valid.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns><c>true</c> if the specified path is valid path; otherwise, <c>false</c>.</returns>
        public static bool IsValidPath(string path) 
        {
            if (string.IsNullOrEmpty(path)) 
            {
                return false;
            }

            path = NormalizePath(path);

            string[] pathChunks = path.Split(PathSeparator.ToCharArray());
            return pathChunks.All(pathChunk => pathChunk.Length != 0);
        }

        /// <summary>
        /// Determines whether the specified path represents the root of the Orchard drive.
        /// </summary>
        /// <param name="path">The path to examine.</param>
        /// <param name="root">The root path of the Orchard drive.</param>
        /// <returns><c>true</c> if the path represents the root path; otherwise, <c>false</c>.</returns>
        public static bool IsDrivePath(string path, string root) 
        {
            if (string.IsNullOrEmpty(path.Replace(root, string.Empty)))
            {
                return true;
            }

            if (string.IsNullOrEmpty(path.Replace(root + PathSeparator, string.Empty)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Joins two paths.
        /// </summary>
        /// <param name="left">The left part of the path.</param>
        /// <param name="right">The right part of the path.</param>
        /// <returns>The joined path.</returns>
        public static string JoinPath(string left, string right)
        {
            if (left == null)
            {
                left = string.Empty;
            }

            if (!left.EndsWith(PathSeparator))
            {
                left += PathSeparator;
            }

            if (right == null)
            {
                right = string.Empty;
            }

            if (right.StartsWith(PathSeparator))
            {
                right = right.Substring(1);
            }

            return left + right;
        }

        /// <summary>
        /// Normalizes the specified path.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>The specified path in normalized form.</returns>
        public static string NormalizePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return path.Replace("/", PathSeparator);
            }

            return path;
        }
    }
}