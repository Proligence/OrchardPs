namespace Proligence.PowerShell.Provider.Vfs.Provider
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
        public VfsProviderException()
        {
        }

        public VfsProviderException(string message) 
            : base(message)
        {
        }

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

        public VfsProviderException(string message, Exception inner) 
            : base(message, inner)
        {
        }

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

        public override void GetObjectData(SerializationInfo info, StreamingContext context) 
        {
            base.GetObjectData(info, context);
            
            info.AddValue("IsFatal", this.IsFatal);
            info.AddValue("ErrorId", this.ErrorId);
            info.AddValue("ErrorCategory", this.ErrorCategory);
        }
    }
}