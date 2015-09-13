using System.Collections;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// Implements a VFS node which maps a name to a single value.
    /// </summary>
    public class PropertyNode : ObjectNode {
        public PropertyNode(PropertyStoreNode parent, string name, object value)
            : base(parent.Vfs, name, new DictionaryEntry(name, value)) {
            SetItemHandler = (node, v) => parent.SetValue(node.Name, v);
            ClearItemHandler = node => parent.SetValue(node.Name, null);
            SetContentHandler = node => new PropertyContentWriter(node.Name, parent);
            GetContentHandler = node => new PropertyContentReader(node.Name, parent);
            ClearContentHandler = node => parent.SetValue(node.Name, null);
        }
    }
}