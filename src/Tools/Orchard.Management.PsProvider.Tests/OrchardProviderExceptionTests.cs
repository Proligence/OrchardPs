// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardProviderExceptionTests.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Tests 
{
    using System;
    using System.IO;
    using System.Management.Automation;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    /// <summary>
    /// Implements unit tests for the <see cref="OrchardProviderException"/> class.
    /// </summary>
    [TestFixture]
    public class OrchardProviderExceptionTests 
    {
        /// <summary>
        /// Tests the constructor of the <see cref="OrchardProviderException"/> class when no parameters are specified.
        /// </summary>
        [Test]
        public void TestConstructor() 
        {
            var exception = new OrchardProviderException();
            
            string expected = "Exception of type 'Orchard.Management.PsProvider.OrchardProviderException' was thrown.";
            Assert.That(exception.Message, Is.EqualTo(expected));
            Assert.IsNull(exception.InnerException);
        }

        /// <summary>
        /// Tests the constructor of the <see cref="OrchardProviderException"/> class when a message is specified.
        /// </summary>
        [Test]
        public void TestConstructorWithMessage() 
        {
            var exception = new OrchardProviderException("My message.");

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Tests the constructor of the <see cref="OrchardProviderException"/> class when a message and inner
        /// exception is specified.
        /// </summary>
        [Test]
        public void TestConstructorWithMessageAndInnerException()
        {
            var inner = new InvalidOperationException("Test exception message");
            var exception = new OrchardProviderException("My message.", inner);

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        }

        /// <summary>
        /// Tests the constructor of the <see cref="OrchardProviderException"/> class when PowerShell-related details
        /// are specified.
        /// </summary>
        [Test]
        public void TestConstructorWithPowerShellDetails()
        {
            var exception = new OrchardProviderException(
                "My message.", true, "ErrorId", ErrorCategory.OperationTimeout);

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.IsFatal, Is.True);
            Assert.That(exception.ErrorId, Is.EqualTo("ErrorId"));
            Assert.That(exception.ErrorCategory, Is.EqualTo(ErrorCategory.OperationTimeout));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Tests the constructor of the <see cref="OrchardProviderException"/> class when PowerShell-related details
        /// and an inner exception are specified.
        /// </summary>
        [Test]
        public void TestConstructorWithPowerShellDetailsAndInnerException()
        {
            var inner = new InvalidOperationException("Test exception message");
            var exception = new OrchardProviderException(
                "My message.", inner, true, "ErrorId", ErrorCategory.OperationTimeout);

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.IsFatal, Is.True);
            Assert.That(exception.ErrorId, Is.EqualTo("ErrorId"));
            Assert.That(exception.ErrorCategory, Is.EqualTo(ErrorCategory.OperationTimeout));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        }

        /// <summary>
        /// Tests if the <see cref="OrchardProviderException"/> class is serialized correctly.
        /// </summary>
        [Test]
        public void TestSerialization()
        {
            var inner = new InvalidOperationException("Test exception message");
            var exception = new OrchardProviderException(
                "My message.", inner, true, "ErrorId", ErrorCategory.OperationTimeout);

            OrchardProviderException deserializedException;
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);

                stream.Position = 0;
                deserializedException = (OrchardProviderException)formatter.Deserialize(stream);
            }

            Assert.That(deserializedException.Message, Is.EqualTo("My message."));
            Assert.That(deserializedException.IsFatal, Is.True);
            Assert.That(deserializedException.ErrorId, Is.EqualTo("ErrorId"));
            Assert.That(deserializedException.ErrorCategory, Is.EqualTo(ErrorCategory.OperationTimeout));
            Assert.That(deserializedException.InnerException.Message, Is.EqualTo(inner.Message));
        }
    }
}