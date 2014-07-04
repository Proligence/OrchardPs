// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Tests
{
    using System;
    using NUnit.Framework;
    using Proligence.PowerShell.Vfs.Utils;

    /// <summary>
    /// Implements unit tests for the <see cref="Utils.ThrowHelper"/> class.
    /// </summary>
    [TestFixture]
    public class ThrowHelperTests
    {
        /// <summary>
        /// Tests the <see cref="Utils.ThrowHelper.InvalidPathException"/> method when a valid path is specified.
        /// </summary>
        [Test]
        public void InvalidPathExceptionWhenValidPath()
        {
            ArgumentException exception = ThrowHelper.InvalidPathException(@"VFS:\x\y\z");

            Assert.That(
                exception.Message,
                Is.StringContaining(@"Path must represent a valid object: VFS:\x\y\z"));
        }

        /// <summary>
        /// Tests the <see cref="Utils.ThrowHelper.InvalidPathException"/> method when the specified path is
        /// <c>null</c> or empty.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase("")]
        [TestCase(null)]
        public void InvalidPathExceptionWhenPathNullOrEmpty(string path)
        {
            ArgumentException exception = ThrowHelper.InvalidPathException(path);

            Assert.That(exception.Message, Is.StringContaining("Path must represent a valid object: "));
        }
    }
}