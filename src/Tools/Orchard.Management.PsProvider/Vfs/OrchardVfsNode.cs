// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardVfsNode.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    using System;

    /// <summary>
    /// The base class for classes which represent nodes in the Orchard Virtual File System (VFS).
    /// </summary>
    public abstract class OrchardVfsNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardVfsNode"/> class.
        /// </summary>
        /// <param name="vfs">The VFS instance which the node belongs to.</param>
        /// <param name="name">The name of the node.</param>
        protected OrchardVfsNode(IOrchardVfs vfs, string name)
        {
            this.Vfs = vfs;
            this.Name = name;
            this.InvokeDefaultActionName = "Invoke default action";
            this.SetItemName = "Set item";
            this.ClearItemName = "Clear item";
            this.CopyItemName = "Copy item";
            this.MoveItemName = "Move item";
            this.RemoveItemName = "Remove item";
            this.RenameItemName = "Rename item";
        }

        /// <summary>
        /// Gets the VFS instance which the node belongs to.
        /// </summary>
        public IOrchardVfs Vfs { get; private set; }

        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the node's parent node.
        /// </summary>
        public OrchardVfsNode Parent { get; protected internal set; }

        /// <summary>
        /// Gets or sets the item encapsulated by the node.
        /// </summary>
        public object Item { get; protected set; }
        
        /// <summary>
        /// Gets or sets the action which implements the <c>Invoke-Item</c> cmdlet.
        /// </summary>
        public Action<OrchardVfsNode> InvokeDefaultActionHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Invoke-Item</c> cmdlet.
        /// </summary>
        public string InvokeDefaultActionName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Set-Item</c> cmdlet.
        /// </summary>
        public Action<OrchardVfsNode, object> SetItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Set-Item</c> cmdlet.
        /// </summary>
        public string SetItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Clear-Item</c> cmdlet.
        /// </summary>
        public Action<OrchardVfsNode> ClearItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Clear-Item</c> cmdlet.
        /// </summary>
        public string ClearItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Copy-Item</c> cmdlet.
        /// </summary>
        public Action<OrchardVfsNode, OrchardVfsNode, bool> CopyItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Copy-Item</c> cmdlet.
        /// </summary>
        public string CopyItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Move-Item</c> cmdlet.
        /// </summary>
        public Action<OrchardVfsNode, OrchardVfsNode> MoveItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Move-Item</c> cmdlet.
        /// </summary>
        public string MoveItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Remove-Item</c> cmdlet.
        /// </summary>
        public Action<OrchardVfsNode, bool> RemoveItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Remove-Item</c> cmdlet.
        /// </summary>
        public string RemoveItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Rename-Item</c> cmdlet.
        /// </summary>
        public Action<OrchardVfsNode, string> RenameItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Rename-Item</c> cmdlet.
        /// </summary>
        public string RenameItemName { get; protected set; }

        /// <summary>
        /// Gets the full path of the node.
        /// </summary>
        /// <returns>The full path of the node.</returns>
        public string GetPath() 
        {
            if (this.Parent == null) 
            {
                return this.Name;
            }

            string path = OrchardPath.JoinPath(this.Parent.GetPath(), this.Name);
            int index = 0;
            while (path[index] == '\\' && index < path.Length) 
            {
                index++;
            }
            
            return path.Substring(index);
        }

        /// <summary>
        /// Gets the child node under the specified relative path.
        /// </summary>
        /// <param name="path">The relative path of the child node to get.</param>
        /// <returns>
        /// The child node under the specified relative path or <c>null</c> if there is no child node under the
        /// specified relative path.
        /// </returns>
        public virtual OrchardVfsNode NavigatePath(string path)
        {
            if (path == null)
            {
                return this;
            }

            if (path.StartsWith("\\"))
            {
                path = path.Substring(1);
            }

            if (path == string.Empty)
            {
                return this;
            }

            return null;
        }
    }
}