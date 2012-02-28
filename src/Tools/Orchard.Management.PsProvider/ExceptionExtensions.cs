﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtensions.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System;

    /// <summary>
    /// Implements extension methods for the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionExtensions 
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
            string message = string.Empty;
            
            while (exception != null) 
            {
                if (!message.Contains(exception.Message))
                {
                    message += exception.Message;
                }

                exception = exception.InnerException;
            }

            return message;
        }
    }
}