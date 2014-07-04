// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionExtensionsTests.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Tests
{
    using System;
    using NUnit.Framework;
    using Proligence.PowerShell.Vfs.Utils;

    /// <summary>
    /// Implements unit tests for the <see cref="ExceptionExtensions"/> class.
    /// </summary>
    [TestFixture]
    public class ExceptionExtensionsTests
    {
        /// <summary>
        /// Tests if the <see cref="ExceptionExtensions.CollectMessages"/> method throws an
        /// <see cref="ArgumentNullException"/> when the specified exception is <c>null</c>.
        /// </summary>
        [Test]
        public void CollectMessagesWhenExceptionNull()
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => ExceptionExtensions.CollectMessages(null));

            Assert.That(exception.ParamName, Is.EqualTo("exception"));
        }

        /// <summary>
        /// Tests if the <see cref="ExceptionExtensions.CollectMessages"/> method works correctly when the specified
        /// exception does not have an inner exception.
        /// </summary>
        [Test]
        public void CollectMessagesWhenExceptionWithoutInnerException()
        {
            var exception = new InvalidOperationException("Test exception message.");

            string result = exception.CollectMessages();

            Assert.That(result, Is.EqualTo("Test exception message."));
        }

        /// <summary>
        /// Tests if the <see cref="ExceptionExtensions.CollectMessages"/> method works correctly when the specified
        /// exception has inner exceptions.
        /// </summary>
        [Test]
        public void CollectMessagesWhenExceptionWithInnerExceptions()
        {
            var exception1 = new InvalidOperationException("Test exception message 1.");
            var exception2 = new InvalidOperationException("Test exception message 2.", exception1);
            var exception3 = new InvalidOperationException("Test exception message 3.", exception2);

            string result = exception3.CollectMessages();

            Assert.That(
                result,
                Is.EqualTo("Test exception message 3. Test exception message 2. Test exception message 1."));
        }

        /// <summary>
        /// Tests if the <see cref="ExceptionExtensions.CollectMessages"/> method works correctly when the message of
        /// the specified exception ends with whitespace.
        /// </summary>
        [Test]
        public void CollectMessagesWhenExceptionMessagesEndsWithWhiteSpace()
        {
            var exception1 = new InvalidOperationException("Test exception message 1.");
            var exception2 = new InvalidOperationException("Test exception message 2.", exception1);

            string result = exception2.CollectMessages();

            Assert.That(result, Is.EqualTo("Test exception message 2. Test exception message 1."));
        }

        /// <summary>
        /// Tests if the <see cref="ExceptionExtensions.CollectMessages"/> method works correctly when the exception
        /// has duplicate messages in the inner exceptions.
        /// </summary>
        [Test]
        public void CollectMessagesWhenDuplicateInnerMessages()
        {
            var exception1 = new InvalidOperationException("Message 2.");
            var exception2 = new InvalidOperationException("Message 1.", exception1);
            var exception3 = new InvalidOperationException("Message 2.", exception2);
            var exception4 = new InvalidOperationException("Message 1.", exception3);
            var exception5 = new InvalidOperationException("Message 1.", exception4);

            string result = exception5.CollectMessages();

            Assert.That(result, Is.EqualTo("Message 1. Message 2."));
        }
    }
}