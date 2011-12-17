using System;

namespace Orchard.Management.PsProvider.Vfs {
    public abstract class OrchardVfsNode {
        protected OrchardVfsNode(string name) {
            Name = name;
            InvokeDefaultActionName = "Invoke default action";
            SetItemName = "Set item";
            ClearItemName = "Clear item";
            CopyItemName = "Copy item";
            MoveItemName = "Move item";
            RemoveItemName = "Remove item";
            RenameItemName = "Rename item";
        }

        public string Name { get; private set; }
        public OrchardVfsNode Parent { get; protected internal set; }
        public object Item { get; protected set; }
        
        public Action<OrchardVfsNode> InvokeDefaultActionHandler { get; protected set; }
        public string InvokeDefaultActionName { get; protected set; }

        public Action<OrchardVfsNode, object> SetItemHandler { get; protected set; }
        public string SetItemName { get; protected set; }

        public Action<OrchardVfsNode> ClearItemHandler { get; protected set; }
        public string ClearItemName { get; protected set; }

        public Action<OrchardVfsNode, OrchardVfsNode, bool> CopyItemHandler { get; protected set; }
        public string CopyItemName { get; protected set; }

        public Action<OrchardVfsNode, OrchardVfsNode> MoveItemHandler { get; protected set; }
        public string MoveItemName { get; protected set; }

        public Action<OrchardVfsNode, bool> RemoveItemHandler { get; protected set; }
        public string RemoveItemName { get; protected set; }

        public Action<OrchardVfsNode, string> RenameItemHandler { get; protected set; }
        public string RenameItemName { get; protected set; }

        public string GetPath() {
            if (Parent == null) {
                return Name;
            }

            string path = OrchardPath.JoinPath(Parent.GetPath(), Name);
            int index = 0;
            while (path[index] == '\\' && index < path.Length) {
                index++;
            }
            
            return path.Substring(index);
        }

        public virtual OrchardVfsNode NavigatePath(string path) {
            if (path == null) {
                return this;
            }

            if (path.StartsWith("\\")) {
                path = path.Substring(1);
            }

            if (path == string.Empty) {
                return this;
            }

            return null;
        }
    }
}