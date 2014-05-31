// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardCmdletExtensionsTests.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Tests
{
    using System;
    using System.Management.Automation;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Implements unit tests for the <see cref="OrchardCmdletExtensions"/> class.
    /// </summary>
    [TestFixture]
    public class OrchardCmdletExtensionsTests
    {
        /// <summary>
        /// Tests if the <see cref="OrchardCmdletExtensions.WriteError"/> method throws an
        /// <see cref="ArgumentNullException"/> when the specified Orchard cmdlet is <c>null</c>.
        /// </summary>
        [Test]
        public void WriteErrorWhenCmdletNull()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => OrchardCmdletExtensions.WriteError(null, ex, "ErrorId", ErrorCategory.InvalidOperation, target));

            Assert.That(exception.ParamName, Is.EqualTo("cmdlet"));
        }

        /// <summary>
        /// Tests if the <see cref="OrchardCmdletExtensions.WriteError"/> method works correctly when the specified
        /// arguments are valid.
        /// </summary>
        [Test]
        public void WriteErrorWhenValidArgs()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            var cmdlet = new Mock<IOrchardCmdlet>();

            ErrorRecord errorRecord = null;
            cmdlet.Setup(x => x.WriteError(It.IsAny<ErrorRecord>()))
                .Callback<ErrorRecord>(er => { errorRecord = er; });

            cmdlet.Object.WriteError(ex, "ErrorId", ErrorCategory.InvalidOperation, target);

            Assert.That(errorRecord, Is.Not.Null);
            Assert.That(errorRecord.Exception, Is.EqualTo(ex));
            Assert.That(errorRecord.FullyQualifiedErrorId, Is.EqualTo("ErrorId"));
            Assert.That(errorRecord.CategoryInfo.Category, Is.EqualTo(ErrorCategory.InvalidOperation));
            Assert.That(errorRecord.TargetObject, Is.SameAs(target));

            cmdlet.VerifyAll();
        }

        /// <summary>
        /// Tests if the <see cref="OrchardCmdletExtensions.ThrowTerminatingError"/> method throws an
        /// <see cref="ArgumentNullException"/> when the specified Orchard cmdlet is <c>null</c>.
        /// </summary>
        [Test]
        public void ThrowTerminatingErrorWhenCmdletNull()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => OrchardCmdletExtensions.ThrowTerminatingError(
                    null, ex, "ErrorId", ErrorCategory.InvalidOperation, target));

            Assert.That(exception.ParamName, Is.EqualTo("cmdlet"));
        }

        /// <summary>
        /// Tests if the <see cref="OrchardCmdletExtensions.ThrowTerminatingError"/> method works correctly when the
        /// specified arguments are valid.
        /// </summary>
        [Test]
        public void ThrowTerminatingErrorWhenValidArgs()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            var cmdlet = new Mock<IOrchardCmdlet>();

            ErrorRecord errorRecord = null;
            cmdlet.Setup(x => x.ThrowTerminatingError(It.IsAny<ErrorRecord>()))
                .Callback<ErrorRecord>(er => { errorRecord = er; });

            cmdlet.Object.ThrowTerminatingError(ex, "ErrorId", ErrorCategory.InvalidOperation, target);

            Assert.That(errorRecord, Is.Not.Null);
            Assert.That(errorRecord.Exception, Is.EqualTo(ex));
            Assert.That(errorRecord.FullyQualifiedErrorId, Is.EqualTo("ErrorId"));
            Assert.That(errorRecord.CategoryInfo.Category, Is.EqualTo(ErrorCategory.InvalidOperation));
            Assert.That(errorRecord.TargetObject, Is.SameAs(target));

            cmdlet.VerifyAll();
        }
    }
}