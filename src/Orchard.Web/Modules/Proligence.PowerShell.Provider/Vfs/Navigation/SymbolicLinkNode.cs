using System.Collections.Generic;
using Orchard.Validation;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// Implements a node which works as a symbolic link (shortcut) to another node in the PowerShell VFS.
    /// </summary>
    public class SymbolicLinkNode : ContainerNode {
        public SymbolicLinkNode(IPowerShellVfs vfs, string name, VfsNode node)
            : base(vfs, name) {
            Argument.ThrowIfNull(node, "node");

            TargetNode = node;
            Item = node.Item;

            InvokeDefaultActionHandler = node.InvokeDefaultActionHandler;
            InvokeDefaultActionName = node.InvokeDefaultActionName;
            SetItemHandler = node.SetItemHandler;
            SetItemName = node.SetItemName;
            ClearItemHandler = node.ClearItemHandler;
            ClearItemName = node.ClearItemName;

            var containerNode = node as ContainerNode;
            if (containerNode != null) {
                NewItemHandler = containerNode.NewItemHandler;
                NewItemName = containerNode.NewItemName;
                CopyItemHandler = containerNode.CopyItemHandler;
                CopyItemName = containerNode.CopyItemName;
                MoveItemHandler = containerNode.MoveItemHandler;
                MoveItemName = containerNode.MoveItemName;
                RemoveItemHandler = containerNode.RemoveItemHandler;
                RemoveItemName = containerNode.RemoveItemName;
                RenameItemHandler = containerNode.RenameItemHandler;
                RenameItemName = containerNode.RemoveItemName;

                foreach (VfsNode staticNode in containerNode.StaticNodes) {
                    AddStaticNode(staticNode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the target node of the symbolic link.
        /// </summary>
        public VfsNode TargetNode { get; protected set; }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() {
            var containerNode = TargetNode as ContainerNode;
            if (containerNode == null) {
                return new VfsNode[0];
            }

            return containerNode.GetVirtualNodes();
        }
    }
}