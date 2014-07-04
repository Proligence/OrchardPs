// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VfsProviderExceptionTests.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Tests 
{
    using System;
    using System.IO;
    using System.Management.Automation;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using Proligence.PowerShell.Vfs.Provider;

    /// <summary>
    /// Implements unit tests for the <see cref="VfsProviderException"/> class.
    /// </summary>
    [TestFixture]
    public class VfsProviderExceptionTests
    {
        /// <summary>
        /// Tests the constructor of the <see cref="VfsProviderException"/> class when no parameters are specified.
        /// </summary>
        [Test]
        public void TestConstructor() 
        {
            var exception = new VfsProviderException();
            
            string expected = "Exception of type 'Proligence.PowerShell.Vfs.Provider.VfsProviderException' was thrown.";
            Assert.That(exception.Message, Is.EqualTo(expected));
            Assert.IsNull(exception.InnerException);
        }

        /// <summary>
        /// Tests the constructor of the <see cref="VfsProviderException"/> class when a message is specified.
        /// </summary>
        [Test]
        public void TestConstructorWithMessage() 
        {
            var exception = new VfsProviderException("My message.");

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Tests the constructor of the <see cref="VfsProviderException"/> class when a message and inner
        /// exception is specified.
        /// </summary>
        [Test]
        public void TestConstructorWithMessageAndInnerException()
        {
            var inner = new InvalidOperationException("Test exception message");
            var exception = new VfsProviderException("My message.", inner);

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        }

        /// <summary>
        /// Tests the constructor of the <see cref="VfsProviderException"/> class when PowerShell-related details
        /// are specified.
        /// </summary>
        [Test]
        public void TestConstructorWithPowerShellDetails()
        {
            var exception = new VfsProviderException(
                "My message.", true, "ErrorId", ErrorCategory.OperationTimeout);

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.IsFatal, Is.True);
            Assert.That(exception.ErrorId, Is.EqualTo("ErrorId"));
            Assert.That(exception.ErrorCategory, Is.EqualTo(ErrorCategory.OperationTimeout));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Tests the constructor of the <see cref="VfsProviderException"/> class when PowerShell-related details
        /// and an inner exception are specified.
        /// </summary>
        [Test]
        public void TestConstructorWithPowerShellDetailsAndInnerException()
        {
            var inner = new InvalidOperationException("Test exception message");
            var exception = new VfsProviderException(
                "My message.", inner, true, "ErrorId", ErrorCategory.OperationTimeout);

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.IsFatal, Is.True);
            Assert.That(exception.ErrorId, Is.EqualTo("ErrorId"));
            Assert.That(exception.ErrorCategory, Is.EqualTo(ErrorCategory.OperationTimeout));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        }

        /// <summary>
        /// Tests if the <see cref="VfsProviderException"/> class is serialized correctly.
        /// </summary>
        [Test]
        public void TestSerialization()
        {
            var inner = new InvalidOperationException("Test exception message");
            var exception = new VfsProviderException(
                "My message.", inner, true, "ErrorId", ErrorCategory.OperationTimeout);

            VfsProviderException deserializedException;
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, exception);

                stream.Position = 0;
                deserializedException = (VfsProviderException)formatter.Deserialize(stream);
            }

            Assert.That(deserializedException.Message, Is.EqualTo("My message."));
            Assert.That(deserializedException.IsFatal, Is.True);
            Assert.That(deserializedException.ErrorId, Is.EqualTo("ErrorId"));
            Assert.That(deserializedException.ErrorCategory, Is.EqualTo(ErrorCategory.OperationTimeout));
            Assert.That(deserializedException.InnerException.Message, Is.EqualTo(inner.Message));
        }
    }
}