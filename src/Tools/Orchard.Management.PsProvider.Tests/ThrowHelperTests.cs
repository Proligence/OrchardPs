// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Implements unit tests for the <see cref="PsProvider.ThrowHelper"/> class.
    /// </summary>
    [TestFixture]
    public class ThrowHelperTests
    {
        /// <summary>
        /// Tests if the <see cref="PsProvider.ThrowHelper.InvalidRootPathException"/> method works correctly when the
        /// specified path is <c>null</c>.
        /// </summary>
        [Test]
        public void InvalidRootPathExceptionWhenPathNull()
        {
            ArgumentException exception = ThrowHelper.InvalidRootPathException(null);

            Assert.That(
                exception.Message,
                Is.EqualTo("The directory '' does not contain an Orchard installation."));
        }

        /// <summary>
        /// Tests if the <see cref="PsProvider.ThrowHelper.InvalidRootPathException"/> method works correctly when the
        /// specified path is not <c>null</c>.
        /// </summary>
        [Test]
        public void InvalidRootPathExceptionWhenPathNotNull()
        {
            ArgumentException exception = ThrowHelper.InvalidRootPathException(@"C:\test");

            Assert.That(
                exception.Message,
                Is.EqualTo(@"The directory 'C:\test' does not contain an Orchard installation."));
        }

        /// <summary>
        /// Tests if the <see cref="PsProvider.ThrowHelper.InvalidPathException"/> method works correctly when the
        /// specified path is <c>null</c>.
        /// </summary>
        [Test]
        public void InvalidPathExceptionWhenPathNull()
        {
            ArgumentException exception = ThrowHelper.InvalidPathException(null);

            Assert.That(exception.Message, Is.EqualTo("Path must represent a valid Orchard object: "));
        }

        /// <summary>
        /// Tests if the <see cref="PsProvider.ThrowHelper.InvalidPathException"/> method works correctly when the
        /// specified path is not <c>null</c>.
        /// </summary>
        [Test]
        public void InvalidPathExceptionWhenPathNotNull()
        {
            ArgumentException exception = ThrowHelper.InvalidPathException(@"Orchard:\Test");

            Assert.That(exception.Message, Is.EqualTo(@"Path must represent a valid Orchard object: Orchard:\Test"));
        }

        /// <summary>
        /// Tests if the <see cref="PsProvider.ThrowHelper.InvalidOperation"/> method works correctly when the
        /// specified message is <c>null</c>.
        /// </summary>
        [Test]
        public void InvalidOperationWhenMessageNull()
        {
            InvalidOperationException exception = ThrowHelper.InvalidOperation(null);

            Assert.That(
                exception.Message,
                Is.EqualTo("Exception of type 'System.InvalidOperationException' was thrown."));
        }

        /// <summary>
        /// Tests if the <see cref="PsProvider.ThrowHelper.InvalidOperation"/> method works correctly when the
        /// specified message is not <c>null</c>.
        /// </summary>
        [Test]
        public void InvalidOperationWhenMessageNotNull()
        {
            InvalidOperationException exception = ThrowHelper.InvalidOperation("Test exception message.");

            Assert.That(exception.Message, Is.EqualTo("Test exception message."));
        }
    }
}