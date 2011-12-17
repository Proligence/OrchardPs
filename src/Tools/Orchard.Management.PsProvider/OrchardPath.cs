using System.Linq;

namespace Orchard.Management.PsProvider {
    internal static class OrchardPath {
        public const string PathSeparator = "\\";

        public static bool IsValidPath(string path) {
            if (string.IsNullOrEmpty(path)) {
                return false;
            }

            path = NormalizePath(path);

            string[] pathChunks = path.Split(PathSeparator.ToCharArray());
            return pathChunks.All(pathChunk => pathChunk.Length != 0);
        }

        public static bool IsDrivePath(string path, string root) {
            if (string.IsNullOrEmpty(path.Replace(root, string.Empty))) {
                return true;
            }

            if (string.IsNullOrEmpty(path.Replace(root + PathSeparator, string.Empty))) {
                return true;
            }

            return false;
        }

        public static string JoinPath(string left, string right) {
            if (left == null) {
                left = string.Empty;
            }

            if (!left.EndsWith(PathSeparator)) {
                left += PathSeparator;
            }

            if (right == null) {
                right = string.Empty;
            }

            if (right.StartsWith(PathSeparator)) {
                right = right.Substring(1);
            }

            return left + right;
        }

        public static string NormalizePath(string path) {
            if (!string.IsNullOrEmpty(path)) {
                return path.Replace("/", PathSeparator);
            }

            return path;
        }
    }
}