// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardVfs.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    using System.Collections.Generic;

    /// <summary>
    /// Implements the Orchard Virtual File System (VFS).
    /// </summary>
    internal class OrchardVfs : IOrchardVfs 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardVfs"/> class.
        /// </summary>
        /// <param name="drive">The object which represents the Orchard drive of the VFS instance.</param>
        /// <param name="navigationProviderManager">The navigation provider manager for the VFS instance.</param>
        public OrchardVfs(OrchardDriveInfo drive, INavigationProviderManager navigationProviderManager)
        {
            this.Root = new RootVfsNode(this, drive);
            this.NavigationProviderManager = navigationProviderManager;
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
        /// Initializes the VFS instance.
        /// </summary>
        public void Initialize() 
        {
            IEnumerable<IPsNavigationProvider> globalNodeProviders = 
                this.NavigationProviderManager.GetProviders(NodeType.Global);

            foreach (IPsNavigationProvider provider in globalNodeProviders) 
            {
                OrchardVfsNode parent = this.NavigatePath(provider.Path);
                if (parent == null) 
                {
                    throw new OrchardProviderException("Orchard VFS path '" + provider.Path + "' does not exist.");
                }

                ContainerNode containerNode = parent as ContainerNode;
                if (containerNode == null) 
                {
                    throw new OrchardProviderException("The node '" + provider.Path + "' must be a container.");
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
        public OrchardVfsNode NavigatePath(string path) 
        {
            if (this.Root != null) 
            {
                return this.Root.NavigatePath(path);
            }

            return null;
        }
    }
}