using System;
using System.Collections;
using System.IO;
using System.Management.Automation.Provider;
using Orchard.Validation;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// Implements the <see cref="IContentWriter"/> for <see cref="PropertyNode"/> VFS nodes.
    /// </summary>
    public sealed class PropertyContentWriter : IContentWriter {
        /// <summary>
        /// The name of the property which will be set by this content writer.
        /// </summary>
        private readonly string _propertyName;

        /// <summary>
        /// The <see cref="PropertyStoreNode"/> which contains the property to set.
        /// </summary>
        private readonly PropertyStoreNode _propertyStoreNode;

        /// <summary>
        /// The value which will be set.
        /// </summary>
        private object _value;

        public PropertyContentWriter(string propertyName, PropertyStoreNode propertyStoreNode) {
            _propertyName = propertyName;
            _propertyStoreNode = propertyStoreNode;
        }

        /// <summary>
        /// Writes the content to the item specified in the GetContentWriter call.
        /// </summary>
        /// <param name="content">
        /// An <see cref="IList"/> collection that contains the blocks to be added to the item.
        /// </param>
        /// <returns>
        /// An <see cref="IList"/> collection that contains the blocks successfully written to the item.
        /// </returns>
        public IList Write(IList content) {
            Argument.ThrowIfNull(content, "content");

            if ((_value != null) || (content.Count != 1)) {
                throw new ArgumentException("Only a single value may be set for property nodes.", "content");
            }

            _value = content[0];
            return content;
        }

        /// <summary>
        /// Moves the content writer to a position in the content.
        /// </summary>
        /// <param name="offset">The offset from the starting point specified by the origin parameter.</param>
        /// <param name="origin">
        /// A <see cref="SeekOrigin"/> enumeration constant that specifies the starting point.
        /// </param>
        public void Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }

        public void Close() {
            _propertyStoreNode.SetValue(_propertyName, _value);
        }

        public void Dispose() {
        }
    }
}