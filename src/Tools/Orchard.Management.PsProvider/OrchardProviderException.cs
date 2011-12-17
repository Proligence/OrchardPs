using System;
using System.Runtime.Serialization;

namespace Orchard.Management.PsProvider {
    [Serializable]
    public class OrchardProviderException : Exception {
        public OrchardProviderException() {
        }

        public OrchardProviderException(string message)
            : base(message) {
        }

        public OrchardProviderException(string message, Exception inner)
            : base(message, inner) {
        }

        protected OrchardProviderException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}