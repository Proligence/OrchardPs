// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardProviderExceptionTests.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Tests 
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Implements unit tests for the <see cref="OrchardProviderException"/> class.
    /// </summary>
    [TestFixture]
    public class OrchardProviderExceptionTests 
    {
        /// <summary>
        /// Tests the constructor of the <see cref="OrchardProvider"/> class when no parameters are specified.
        /// </summary>
        [Test]
        public void TestCtor() 
        {
            var exception = new OrchardProviderException();
            
            string expected = "Exception of type 'Orchard.Management.PsProvider.OrchardProviderException' was thrown.";
            Assert.That(exception.Message, Is.EqualTo(expected));
            Assert.IsNull(exception.InnerException);
        }

        /// <summary>
        /// Tests the constructor of the <see cref="OrchardProvider"/> class when a message is specified.
        /// </summary>
        [Test]
        public void TestCtorWithMessage() 
        {
            var exception = new OrchardProviderException("My message.");

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Tests the constructor of the <see cref="OrchardProvider"/> class when a message and inner exception is
        /// specified.
        /// </summary>
        [Test]
        public void TestCtorWithMessageAndInnerException() 
        {
            var inner = new Exception();
            var exception = new OrchardProviderException("My message.", inner);

            Assert.That(exception.Message, Is.EqualTo("My message."));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        }
    }
}