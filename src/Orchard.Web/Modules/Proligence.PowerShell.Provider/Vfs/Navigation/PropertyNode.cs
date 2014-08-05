namespace Proligence.PowerShell.Provider.Vfs.Navigation
{
    using System.Collections;

    /// <summary>
    /// Implements a VFS node which maps a name to a single value.
    /// </summary>
    public class PropertyNode : ObjectNode
    {
        public PropertyNode(PropertyStoreNode parent, string name, object value)
            : base(parent.Vfs, name, new DictionaryEntry(name, value))
        {
            this.SetItemHandler = (node, v) => parent.SetValue(node.Name, v);
            this.ClearItemHandler = node => parent.SetValue(node.Name, null);
            this.SetContentHandler = node => new PropertyContentWriter(node.Name, parent);
            this.GetContentHandler = node => new PropertyContentReader(node.Name, parent);
            this.ClearContentHandler = node => parent.SetValue(node.Name, null);
        }
    }
}