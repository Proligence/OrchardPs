﻿using System;
using System.Management.Automation.Provider;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// The base class for classes which represent nodes in the PowerShell Virtual File System (VFS).
    /// </summary>
    public abstract class VfsNode {
        protected VfsNode(IPowerShellVfs vfs, string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                name = GetType().Name;
            }

            Vfs = vfs;
            Name = name;
            InvokeDefaultActionName = "Invoke default action";
            SetItemName = "Set item";
            ClearItemName = "Clear item";
            CopyItemName = "Copy item";
            MoveItemName = "Move item";
            RemoveItemName = "Remove item";
            RenameItemName = "Rename item";
            GetContentName = "Get content";
            SetContentName = "Set content";
            ClearContentName = "Clear content";
        }

        /// <summary>
        /// Gets the VFS instance which the node belongs to.
        /// </summary>
        public IPowerShellVfs Vfs { get; private set; }

        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the node's parent node.
        /// </summary>
        public VfsNode Parent { get; set; }

        /// <summary>
        /// Gets or sets the item encapsulated by the node.
        /// </summary>
        public object Item { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Invoke-Item</c> cmdlet.
        /// </summary>
        public Action<VfsNode> InvokeDefaultActionHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Invoke-Item</c> cmdlet.
        /// </summary>
        public string InvokeDefaultActionName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Set-Item</c> cmdlet.
        /// </summary>
        public Action<VfsNode, object> SetItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Set-Item</c> cmdlet.
        /// </summary>
        public string SetItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Clear-Item</c> cmdlet.
        /// </summary>
        public Action<VfsNode> ClearItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Clear-Item</c> cmdlet.
        /// </summary>
        public string ClearItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Copy-Item</c> cmdlet.
        /// </summary>
        public Action<VfsNode, VfsNode, bool> CopyItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Copy-Item</c> cmdlet.
        /// </summary>
        public string CopyItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Move-Item</c> cmdlet.
        /// </summary>
        public Action<VfsNode, VfsNode> MoveItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Move-Item</c> cmdlet.
        /// </summary>
        public string MoveItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Remove-Item</c> cmdlet.
        /// </summary>
        public Action<VfsNode, bool> RemoveItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Remove-Item</c> cmdlet.
        /// </summary>
        public string RemoveItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Rename-Item</c> cmdlet.
        /// </summary>
        public Action<VfsNode, string> RenameItemHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Rename-Item</c> cmdlet.
        /// </summary>
        public string RenameItemName { get; protected set; }

        /// <summary>
        /// Gets or sets the function which implements the <c>Get-Content</c> cmdlet.
        /// </summary>
        public Func<VfsNode, IContentReader> GetContentHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Get-Content</c> cmdlet.
        /// </summary>
        public string GetContentName { get; protected set; }

        /// <summary>
        /// Gets or sets the function which returns an object with additional parameters for the <c>Get-Content</c>
        /// cmdlet.
        /// </summary>
        public Func<VfsNode, object> GetContentDynamicParametersHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the function which implements the <c>Set-Content</c> cmdlet.
        /// </summary>
        public Func<VfsNode, IContentWriter> SetContentHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Set-Content</c> cmdlet.
        /// </summary>
        public string SetContentName { get; protected set; }

        /// <summary>
        /// Gets or sets the function which returns an object with additional parameters for the <c>Set-Content</c>
        /// cmdlet.
        /// </summary>
        public Func<VfsNode, object> SetContentDynamicParametersHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the action which implements the <c>Clear-Content</c> cmdlet.
        /// </summary>
        public Action<VfsNode> ClearContentHandler { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the action performed by the <c>Clear-Content</c> cmdlet.
        /// </summary>
        public string ClearContentName { get; protected set; }

        /// <summary>
        /// Gets or sets the function which returns an object with additional parameters for the <c>Clear-Content</c>
        /// cmdlet.
        /// </summary>
        public Func<VfsNode, object> ClearContentDynamicParametersHandler { get; protected set; }

        /// <summary>
        /// Gets the full path of the node.
        /// </summary>
        /// <returns>The full path of the node.</returns>
        public string GetPath() {
            if (Parent == null) {
                return Name;
            }

            string path = Vfs.PathValidator.JoinPath(Parent.GetPath(), Name);
            int index = 0;
            while (path[index] == '\\' && index < path.Length) {
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
        public virtual VfsNode NavigatePath(string path) {
            if (path == null) {
                return this;
            }

            if (path.StartsWith("\\", StringComparison.Ordinal)) {
                path = path.Substring(1);
            }

            if (string.IsNullOrEmpty(path)) {
                return this;
            }

            return null;
        }
    }
}