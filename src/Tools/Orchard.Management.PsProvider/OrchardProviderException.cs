namespace Orchard.Management.PsProvider
{
    using System;
    using System.Management.Automation;
    using System.Runtime.Serialization;
    using Orchard.Management.PsProvider.Vfs.Provider;

    /// <summary>
    /// Implements an exception which represent errors reported by the Orchard PS provider.
    /// </summary>
    [Serializable]
    public class OrchardProviderException : VfsProviderException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderException"/> class.
        /// </summary>
        public OrchardProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderException"/> class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        public OrchardProviderException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderException"/> class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="fatal">if set to <c>true</c> indicates that the error is fatal.</param>
        /// <param name="errorId">The error identifier.</param>
        /// <param name="category">The error category.</param>
        public OrchardProviderException(
            string message, 
            bool fatal, 
            string errorId, 
            ErrorCategory category = ErrorCategory.NotSpecified) 
            : base(message, fatal, errorId, category) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderException"/> class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="inner">The inner exception.</param>
        public OrchardProviderException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderException"/> class.
        /// </summary>
        /// <param name="message">The exception's message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="fatal">if set to <c>true</c> indicates that the error is fatal.</param>
        /// <param name="errorId">The error identifier.</param>
        /// <param name="category">The error category.</param>
        public OrchardProviderException(
            string message, 
            Exception inner, 
            bool fatal, 
            string errorId, 
            ErrorCategory category = ErrorCategory.NotSpecified) 
            : base(message, inner, fatal, errorId, category) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardProviderException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected OrchardProviderException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        {
        }
    }
}