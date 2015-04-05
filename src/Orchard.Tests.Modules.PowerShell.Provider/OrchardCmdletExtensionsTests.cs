namespace Orchard.Tests.Modules.PowerShell.Provider
{
    using System;
    using System.Management.Automation;
    using Moq;
    using Proligence.PowerShell.Provider;
    using Xunit;

    public class OrchardCmdletExtensionsTests
    {
        [Fact]
        public void WriteErrorWhenCmdletNull()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            var exception = Assert.Throws<ArgumentNullException>(
                () => OrchardCmdletExtensions.WriteError(null, ex, "ErrorId", ErrorCategory.InvalidOperation, target));

            Assert.Equal("cmdlet", exception.ParamName);
        }

        [Fact]
        public void WriteErrorWhenValidArgs()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            var cmdlet = new Mock<IOrchardCmdlet>();

            ErrorRecord errorRecord = null;
            cmdlet.Setup(x => x.WriteError(It.IsAny<ErrorRecord>()))
                .Callback<ErrorRecord>(er => { errorRecord = er; });

            cmdlet.Object.WriteError(ex, "ErrorId", ErrorCategory.InvalidOperation, target);

            Assert.NotNull(errorRecord);
            Assert.Equal(ex, errorRecord.Exception);
            Assert.Equal("ErrorId", errorRecord.FullyQualifiedErrorId);
            Assert.Equal(ErrorCategory.InvalidOperation, errorRecord.CategoryInfo.Category);
            Assert.Same(target, errorRecord.TargetObject);

            cmdlet.VerifyAll();
        }

        [Fact]
        public void ThrowTerminatingErrorWhenCmdletNull()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            var exception = Assert.Throws<ArgumentNullException>(
                () => OrchardCmdletExtensions.ThrowTerminatingError(
                    null, ex, "ErrorId", ErrorCategory.InvalidOperation, target));

            Assert.Equal("cmdlet", exception.ParamName);
        }

        [Fact]
        public void ThrowTerminatingErrorWhenValidArgs()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            var cmdlet = new Mock<IOrchardCmdlet>();

            ErrorRecord errorRecord = null;
            cmdlet.Setup(x => x.ThrowTerminatingError(It.IsAny<ErrorRecord>()))
                .Callback<ErrorRecord>(er => { errorRecord = er; });

            cmdlet.Object.ThrowTerminatingError(ex, "ErrorId", ErrorCategory.InvalidOperation, target);

            Assert.NotNull(errorRecord);
            Assert.Equal(ex, errorRecord.Exception);
            Assert.Equal("ErrorId", errorRecord.FullyQualifiedErrorId);
            Assert.Equal(ErrorCategory.InvalidOperation, errorRecord.CategoryInfo.Category);
            Assert.Same(target, errorRecord.TargetObject);

            cmdlet.VerifyAll();
        }
    }
}