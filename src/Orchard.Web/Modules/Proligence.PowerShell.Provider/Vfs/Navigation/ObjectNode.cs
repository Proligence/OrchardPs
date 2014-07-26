namespace Proligence.PowerShell.Provider.Vfs.Navigation
{
    using Proligence.PowerShell.Provider.Vfs.Core;

    /// <summary>
    /// The base class for VFS nodes which encapsulate item objects.
    /// </summary>
    public class ObjectNode : VfsNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectNode"/> class.
        /// </summary>
        /// <param name="vfs">The PowerShell VFS instance which the node belongs to.</param>
        /// <param name="name">The name of the node.</param>
        /// <param name="obj">The encapsulated item object.</param>
        public ObjectNode(IPowerShellVfs vfs, string name, object obj) 
            : base(vfs, name) 
        {
            this.Item = obj;
        }
    }
}