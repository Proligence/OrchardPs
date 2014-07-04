// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultPathValidator.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Navigation
{
    using System;
    using System.Linq;

    /// <summary>
    /// Validate input paths for VFS-based PS providers.
    /// </summary>
    public class DefaultPathValidator : IPathValidator
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
        public virtual bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            path = this.NormalizePath(path);

            string[] pathChunks = path.Split(PathSeparator.ToCharArray());
            
            if (pathChunks.Any(pathChunk => pathChunk.Length == 0))
            {
                return false;
            }

            for (int i = 0; i < pathChunks.Length; i++)
            {
                if (pathChunks[i].Contains(":"))
                {
                    if (i > 0)
                    {
                        return false;
                    }

                    if (pathChunks[i].StartsWith(":", StringComparison.Ordinal))
                    {
                        return false;
                    }

                    if (pathChunks[i].Count(chr => chr == ':') > 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified path represents the root of the VFS drive.
        /// </summary>
        /// <param name="path">The path to examine.</param>
        /// <param name="root">The root path of the VFS drive.</param>
        /// <returns><c>true</c> if the path represents the root path; otherwise, <c>false</c>.</returns>
        public virtual bool IsDrivePath(string path, string root)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (string.IsNullOrEmpty(root))
            {
                return false;
            }

            // Do case-insensitive search.
            path = path.ToUpperInvariant();
            root = root.ToUpperInvariant();
            
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
        /// Joins two normalized paths.
        /// </summary>
        /// <param name="left">The left part of the path.</param>
        /// <param name="right">The right part of the path.</param>
        /// <returns>The joined path.</returns>
        public virtual string JoinPath(string left, string right)
        {
            if (left == null)
            {
                return right ?? string.Empty;
            }
            
            if (right == null)
            {
                return left;
            }

            if (!left.EndsWith(PathSeparator, StringComparison.Ordinal))
            {
                left += PathSeparator;
            }

            if (right.StartsWith(PathSeparator, StringComparison.Ordinal))
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
        public virtual string NormalizePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace("/", PathSeparator);

                if (path.EndsWith(PathSeparator, StringComparison.Ordinal))
                {
                    path = path.Substring(0, path.Length - PathSeparator.Length);
                }
            }

            return path;
        }
    }
}