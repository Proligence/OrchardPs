namespace Proligence.PowerShell.Provider.Tests
{
    using System;
    using NUnit.Framework;
    using ThrowHelper = Proligence.PowerShell.Provider.Internal.ThrowHelper;

    [TestFixture]
    public class ThrowHelperTests
    {
        [Test]
        public void InvalidRootPathExceptionWhenPathNull()
        {
            ArgumentException exception = ThrowHelper.InvalidRootPathException(null);

            Assert.That(
                exception.Message,
                Is.EqualTo("The directory '' does not contain an Orchard installation."));
        }

        [Test]
        public void InvalidRootPathExceptionWhenPathNotNull()
        {
            ArgumentException exception = ThrowHelper.InvalidRootPathException(@"C:\test");

            Assert.That(
                exception.Message,
                Is.EqualTo(@"The directory 'C:\test' does not contain an Orchard installation."));
        }

        [Test]
        public void InvalidPathExceptionWhenPathNull()
        {
            ArgumentException exception = ThrowHelper.InvalidPathException(null);

            Assert.That(exception.Message, Is.EqualTo("Path must represent a valid Orchard object: "));
        }

        [Test]
        public void InvalidPathExceptionWhenPathNotNull()
        {
            ArgumentException exception = ThrowHelper.InvalidPathException(@"Orchard:\Test");

            Assert.That(exception.Message, Is.EqualTo(@"Path must represent a valid Orchard object: Orchard:\Test"));
        }

        [Test]
        public void InvalidOperationWhenMessageNull()
        {
            InvalidOperationException exception = ThrowHelper.InvalidOperation(null);

            Assert.That(
                exception.Message,
                Is.EqualTo("Exception of type 'System.InvalidOperationException' was thrown."));
        }

        [Test]
        public void InvalidOperationWhenMessageNotNull()
        {
            InvalidOperationException exception = ThrowHelper.InvalidOperation("Test exception message.");

            Assert.That(exception.Message, Is.EqualTo("Test exception message."));
        }
    }
}