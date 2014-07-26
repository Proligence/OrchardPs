namespace Proligence.PowerShell.Provider.Vfs.Navigation
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Management.Automation.Provider;

    /// <summary>
    /// Implements the <see cref="IContentWriter"/> for <see cref="PropertyNode"/> VFS nodes.
    /// </summary>
    public sealed class PropertyContentWriter : IContentWriter
    {
        /// <summary>
        /// The name of the property which will be set by this content writer.
        /// </summary>
        private readonly string propertyName;

        /// <summary>
        /// The <see cref="PropertyStoreNode"/> which contains the property to set.
        /// </summary>
        private readonly PropertyStoreNode propertyStoreNode;
        
        /// <summary>
        /// The value which will be set.
        /// </summary>
        private object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyContentWriter"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property which will be set by this content writer.</param>
        /// <param name="propertyStoreNode">
        /// The <see cref="PropertyStoreNode"/> which contains the property to set.
        /// </param>
        public PropertyContentWriter(string propertyName, PropertyStoreNode propertyStoreNode)
        {
            this.propertyName = propertyName;
            this.propertyStoreNode = propertyStoreNode;
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
        public IList Write(IList content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            if ((this.value != null) || (content.Count != 1))
            {
                throw new ArgumentException("Only a single value may be set for property nodes.", "content");
            }

            this.value = content[0];
            return content;
        }

        /// <summary>
        /// Moves the content writer to a position in the content.
        /// </summary>
        /// <param name="offset">The offset from the starting point specified by the origin parameter.</param>
        /// <param name="origin">
        /// A <see cref="SeekOrigin"/> enumeration constant that specifies the starting point.
        /// </param>
        public void Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Closes the content writer.
        /// </summary>
        public void Close()
        {
            this.propertyStoreNode.SetValue(this.propertyName, this.value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}