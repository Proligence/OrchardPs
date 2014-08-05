namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Management.Automation;
    using System.Runtime.Serialization;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Implements an exception which represent errors reported by the Orchard PS provider.
    /// </summary>
    [Serializable]
    public class OrchardProviderException : VfsProviderException
    {
        public OrchardProviderException()
        {
        }

        public OrchardProviderException(string message) 
            : base(message)
        {
        }

        public OrchardProviderException(
            string message, 
            bool fatal, 
            string errorId, 
            ErrorCategory category = ErrorCategory.NotSpecified) 
            : base(message, fatal, errorId, category) 
        {
        }

        public OrchardProviderException(string message, Exception inner) 
            : base(message, inner)
        {
        }

        public OrchardProviderException(
            string message, 
            Exception inner, 
            bool fatal, 
            string errorId, 
            ErrorCategory category = ErrorCategory.NotSpecified) 
            : base(message, inner, fatal, errorId, category) 
        {
        }

        protected OrchardProviderException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        {
        }
    }
}