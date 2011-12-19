using System.Collections.Generic;

namespace Orchard.Management.PsProvider.Vfs {
    public class SymbolicLinkNode : ContainerNode {
        public SymbolicLinkNode(IOrchardVfs vfs, string name, OrchardVfsNode node) : base(vfs, name) {
            TargetNode = node;
            Item = node.Item;
            
            InvokeDefaultActionHandler = node.InvokeDefaultActionHandler;
            InvokeDefaultActionName = node.InvokeDefaultActionName;
            SetItemHandler = node.SetItemHandler;
            SetItemName = node.SetItemName;
            ClearItemHandler = node.ClearItemHandler;
            ClearItemName = node.ClearItemName;

            ContainerNode containerNode = node as ContainerNode;
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

                foreach (OrchardVfsNode staticNode in containerNode.StaticNodes) {
                    AddStaticNode(staticNode);
                }
            }
        }

        public OrchardVfsNode TargetNode { get; protected set; }

        public override IEnumerable<OrchardVfsNode> GetVirtualNodes() {
            ContainerNode containerNode = TargetNode as ContainerNode;
            if (containerNode == null) {
                return new OrchardVfsNode[0];
            }

            return containerNode.GetVirtualNodes();
        }
    }
}