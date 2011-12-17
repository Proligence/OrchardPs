using System;

namespace Orchard.Management.PsProvider {
    internal static class ThrowHelper {
        public static ArgumentException InvalidRootPathException(string rootPath) {
            return new ArgumentException("The directory '" + rootPath + "' does not contain an Orchard installation.");
        }

        public static ArgumentException InvalidPathException(string path) {
            return new ArgumentException("Path must represent a valid Orchard object: " + path);
        }
    }
}