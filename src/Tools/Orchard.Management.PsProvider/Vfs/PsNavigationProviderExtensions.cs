namespace Orchard.Management.PsProvider.Vfs {
    public static class PsNavigationProviderExtensions {
        public static int GetPathLength(this IPsNavigationProvider navigationProvider) {
            if (!string.IsNullOrEmpty(navigationProvider.Path)) {
                return navigationProvider.Path.Split('\\').Length;
            }

            return 0;
        }
    }
}