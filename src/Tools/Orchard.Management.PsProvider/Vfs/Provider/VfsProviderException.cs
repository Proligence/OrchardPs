namespace Orchard.Management.PsProvider.Vfs.Provider
{
    using System;
    using System.Management.Automation;
    using System.Runtime.Serialization;

    /// <summary>
    /// The base class for exceptions which represent errors from the PowerShell VFS PS provider.
    /// </summary>
    [Serializable]
    public class VfsProviderException : Exception 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VfsProviderException"/> class.
        /// </summary>
        public VfsProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VfsProviderException"/> class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public VfsProviderException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VfsProviderException"/> class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="fatal">if set to <c>true</c> indicates that the error is fatal.</param>
        /// <param name="errorId">The error identifier.</param>
        /// <param name="category">The error category.</param>
        public VfsProviderException(
            string message, 
            bool fatal, 
            string errorId, 
            ErrorCategory category = ErrorCategory.NotSpecified) 
            : base(message) 
        {
            this.IsFatal = fatal;
            this.ErrorId = errorId;
            this.ErrorCategory = category;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VfsProviderException"/> class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="inner">The inner exception.</param>
        public VfsProviderException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VfsProviderException"/> class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="fatal">if set to <c>true</c> indicates that the error is fatal.</param>
        /// <param name="errorId">The error identifier.</param>
        /// <param name="category">The error category.</param>
        public VfsProviderException(
            string message, 
            Exception inner, 
            bool fatal, 
            string errorId, 
            ErrorCategory category = ErrorCategory.NotSpecified) 
            : base(message, inner) 
        {
            this.IsFatal = fatal;
            this.ErrorId = errorId;
            this.ErrorCategory = category;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VfsProviderException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected VfsProviderException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        {
            this.IsFatal = info.GetBoolean("IsFatal");
            this.ErrorId = info.GetString("ErrorId");
            this.ErrorCategory = (ErrorCategory)info.GetValue("ErrorCategory", typeof(ErrorCategory));
        }

        /// <summary>
        /// Gets a value indicating whether the exception represents a fatal error.
        /// </summary>
        public bool IsFatal { get; private set; }

        /// <summary>
        /// Gets the error identifier.
        /// </summary>
        public string ErrorId { get; private set; }

        /// <summary>
        /// Gets the error category.
        /// </summary>
        public ErrorCategory ErrorCategory { get; private set; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo"/> with information about the
        /// exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context) 
        {
            base.GetObjectData(info, context);
            
            info.AddValue("IsFatal", this.IsFatal);
            info.AddValue("ErrorId", this.ErrorId);
            info.AddValue("ErrorCategory", this.ErrorCategory);
        }
    }
}