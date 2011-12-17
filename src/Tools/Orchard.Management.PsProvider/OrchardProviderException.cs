using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Orchard.Management.PsProvider {
    [Serializable]
    public class OrchardProviderException : Exception {
        public bool IsFatal { get; private set; }
        public string ErrorId { get; private set; }
        public ErrorCategory ErrorCategory { get; private set; }

        public OrchardProviderException() { }
        
        public OrchardProviderException(string message) 
            : base(message) { }

        public OrchardProviderException(string message, bool isFatal, string errorId, ErrorCategory category = ErrorCategory.NotSpecified) 
            : base (message) {

            IsFatal = isFatal;
            ErrorId = errorId;
            ErrorCategory = category;
        }

        public OrchardProviderException(string message, Exception inner) 
            : base(message, inner) { }

        public OrchardProviderException(string message, Exception inner, bool isFatal, string errorId, ErrorCategory category = ErrorCategory.NotSpecified) 
            : base(message, inner) {

            IsFatal = isFatal;
            ErrorId = errorId;
            ErrorCategory = category;
        }

        protected OrchardProviderException(SerializationInfo info, StreamingContext context) 
            : base(info, context) {

            IsFatal = info.GetBoolean("IsFatal");
            ErrorId = info.GetString("ErrorId");
            ErrorCategory = (ErrorCategory)info.GetValue("ErrorCategory", typeof(ErrorCategory));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            
            info.AddValue("IsFatal", IsFatal);
            info.AddValue("ErrorId", ErrorId);
            info.AddValue("ErrorCategory", ErrorCategory);
        }
    }
}