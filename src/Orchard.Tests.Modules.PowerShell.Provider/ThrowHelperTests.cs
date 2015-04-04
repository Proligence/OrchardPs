namespace Proligence.PowerShell.Provider.Tests
{
    using System;
    using Xunit;
    using ThrowHelper = Proligence.PowerShell.Provider.Internal.ThrowHelper;

    public class ThrowHelperTests
    {
        [Fact]
        public void InvalidRootPathExceptionWhenPathNull()
        {
            ArgumentException exception = ThrowHelper.InvalidRootPathException(null);

            Assert.Equal("The directory '' does not contain an Orchard installation.", exception.Message);
        }

        [Fact]
        public void InvalidRootPathExceptionWhenPathNotNull()
        {
            ArgumentException exception = ThrowHelper.InvalidRootPathException(@"C:\test");

            Assert.Equal(@"The directory 'C:\test' does not contain an Orchard installation.", exception.Message);
        }

        [Fact]
        public void InvalidPathExceptionWhenPathNull()
        {
            ArgumentException exception = ThrowHelper.InvalidPathException(null);

            Assert.Equal("Path must represent a valid Orchard object: ", exception.Message);
        }

        [Fact]
        public void InvalidPathExceptionWhenPathNotNull()
        {
            ArgumentException exception = ThrowHelper.InvalidPathException(@"Orchard:\Test");

            Assert.Equal(@"Path must represent a valid Orchard object: Orchard:\Test", exception.Message);
        }

        [Fact]
        public void InvalidOperationWhenMessageNull()
        {
            InvalidOperationException exception = ThrowHelper.InvalidOperation(null);

            Assert.Equal("Exception of type 'System.InvalidOperationException' was thrown.", exception.Message);
        }

        [Fact]
        public void InvalidOperationWhenMessageNotNull()
        {
            InvalidOperationException exception = ThrowHelper.InvalidOperation("Test exception message.");

            Assert.Equal("Test exception message.", exception.Message);
        }
    }
}