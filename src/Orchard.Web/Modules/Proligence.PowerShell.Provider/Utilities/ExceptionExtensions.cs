namespace Proligence.PowerShell.Provider.Utilities
{
    using System;

    internal static class ExceptionExtensions 
    {
        /// <summary>
        /// Collects all messages from the specified exception and its inner exceptions.
        /// </summary>
        /// <param name="exception">The exception object.</param>
        /// <returns>
        /// The <see cref="string"/> which contain messages from the exception and its inner exceptions.
        /// </returns>
        public static string CollectMessages(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            string message = string.Empty;
            
            while (exception != null)
            {
                if (!message.Contains(exception.Message))
                {
                    if ((message.Length > 0) && !char.IsWhiteSpace(message[message.Length - 1]))
                    {
                        message += " ";
                    }

                    message += exception.Message;
                }

                exception = exception.InnerException;
            }

            return message;
        }
    }
}