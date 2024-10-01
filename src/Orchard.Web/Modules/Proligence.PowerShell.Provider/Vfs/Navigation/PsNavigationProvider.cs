namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// The base class for classes which extend the PowerShell Virtual File System by adding new static nodes to the
    /// VFS tree.
    /// </summary>
    public abstract class PsNavigationProvider : IPsNavigationProvider {
        protected PsNavigationProvider(NodeType nodeType) {
            NodeType = nodeType;
            Path = "\\";
        }

        protected PsNavigationProvider(NodeType nodeType, VfsNode node) {
            Path = "\\";
            NodeType = nodeType;
            Node = node;
        }

        protected PsNavigationProvider(NodeType nodeType, string path, VfsNode node) {
            Path = path;
            NodeType = nodeType;
            Node = node;
        }

        /// <summary>
        /// Gets or sets the path of the added node's parent node.
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// Gets or sets the type of the added node.
        /// </summary>
        public NodeType NodeType { get; private set; }

        /// <summary>
        /// Gets or sets the added node.
        /// </summary>
        public VfsNode Node { get; protected set; }

        /// <summary>
        /// Gets or sets the PowerShell VFS instance.
        /// </summary>
        public IPowerShellVfs Vfs { get; set; }

        /// <summary>
        /// Gets a value indicating whether this navigation provider is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Initializes the properties of the navigation provider.
        /// </summary>
        public void Initialize() {
            if (!IsInitialized) {
                InitializeInternal();
                IsInitialized = true;
            }
        }

        protected virtual void InitializeInternal() {
        }
    }
}