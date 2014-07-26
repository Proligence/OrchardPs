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
        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellVfs"/> class.
        /// </summary>
        /// <param name="drive">The object which represents the drive of the VFS instance.</param>
        /// <param name="navigationProviderManager">The navigation provider manager for the VFS instance.</param>
        /// <param name="pathValidator">The object which implements input path validation.</param>
        public PowerShellVfs(
            VfsDriveInfo drive,
            INavigationProviderManager navigationProviderManager,
            IPathValidator pathValidator)
        {
            this.Root = new RootVfsNode(this, drive);
            this.NavigationProviderManager = navigationProviderManager;
            this.PathValidator = pathValidator;
        }

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
            IEnumerable<IPsNavigationProvider> globalNodeProviders = 
                this.NavigationProviderManager.GetProviders(NodeType.Global);

            foreach (IPsNavigationProvider provider in globalNodeProviders) 
            {
                VfsNode parent = this.NavigatePath(provider.Path);
                if (parent == null) 
                {
                    throw new VfsProviderException("The VFS path '" + provider.Path + "' does not exist.");
                }

                ContainerNode containerNode = parent as ContainerNode;
                if (containerNode == null) 
                {
                    throw new VfsProviderException("The node '" + provider.Path + "' must be a container.");
                }

                containerNode.AddStaticNode(provider.Node);
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