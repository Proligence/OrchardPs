using System;

namespace Orchard.Management.PsProvider {
    public static class ExceptionExtensions {
        public static string CollectMessages(this Exception exception) {
            string message = string.Empty;
            
            while (exception != null) {
                if (!message.Contains(exception.Message)) {
                    message += exception.Message;
                }

                exception = exception.InnerException;
            }

            return message;
        }
    }
}