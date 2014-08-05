namespace Proligence.PowerShell.Provider.Vfs.Core
{
    using System.Collections.Generic;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Implements the PowerShell Virtual File System (VFS).
    /// </summary>
    public class PowerShellVfs : IPowerShellVfs
    {
        private readonly object initializationLock = new object();
        private bool initialized;

        public PowerShellVfs(
            VfsDriveInfo drive,
            INavigationProviderManager navigationProviderManager,
            IPathValidator pathValidator)
        {
            this.Drive = drive;
            this.Root = new RootVfsNode(this, drive);
            this.NavigationProviderManager = navigationProviderManager;
            this.PathValidator = pathValidator;
        }

        /// <summary>
        /// Gets the Orchard drive instance.
        /// </summary>
        public VfsDriveInfo Drive { get; private set; }

        /// <summary>
        /// Gets the root node of the VFS.
        /// </summary>
        public RootVfsNode Root { get; private set; }

        /// <summary>
        /// Gets the navigation provider manager for the VFS instance.
        /// </summary>
        public INavigationProviderManager NavigationProviderManager { get; private set; }

        /// <summary>
        /// Gets the object which implements input path validation.
        /// </summary>
        public IPathValidator PathValidator { get; private set; }

        /// <summary>
        /// Initializes the VFS instance.
        /// </summary>
        public void Initialize() 
        {
            lock (this.initializationLock)
            {
                if (!this.initialized)
                {
                    IEnumerable<IPsNavigationProvider> globalNodeProviders =
                        this.NavigationProviderManager.GetProviders(NodeType.Global);

                    foreach (IPsNavigationProvider provider in globalNodeProviders)
                    {
                        provider.Vfs = this;
                        provider.Initialize();

                        VfsNode parent = this.NavigatePath(provider.Path);
                        if (parent == null)
                        {
                            throw new VfsProviderException("The VFS path '" + provider.Path + "' does not exist.");
                        }

                        var containerNode = parent as ContainerNode;
                        if (containerNode == null)
                        {
                            throw new VfsProviderException("The node '" + provider.Path + "' must be a container.");
                        }

                        containerNode.AddStaticNode(provider.Node);
                    }

                    this.initialized = true;
                }
            }
        }

        /// <summary>
        /// Gets the node under the specified path in the VFS.
        /// </summary>
        /// <param name="path">The path of the node to get.</param>
        /// <returns>
        /// The node under the specified path or <c>null</c> if there is no node under the specified path.
        /// </returns>
        public VfsNode NavigatePath(string path) 
        {
            if (this.Root != null) 
            {
                return this.Root.NavigatePath(path);
            }

            return null;
        }
    }
}