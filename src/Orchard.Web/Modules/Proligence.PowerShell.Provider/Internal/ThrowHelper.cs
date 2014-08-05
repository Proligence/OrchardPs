namespace Proligence.PowerShell.Provider.Internal
{
    using System;

    /// <summary>
    /// Implements helper methods for creating exception objects.
    /// </summary>
    internal static class ThrowHelper 
    {
        public static ArgumentException InvalidRootPathException(string rootPath)
        {
            return new ArgumentException("The directory '" + rootPath + "' does not contain an Orchard installation.");
        }

        public static ArgumentException InvalidPathException(string path) 
        {
            return new ArgumentException("Path must represent a valid Orchard object: " + path);
        }

        public static InvalidOperationException InvalidOperation(string message) 
        {
            return new InvalidOperationException(message);
        }
    }
}