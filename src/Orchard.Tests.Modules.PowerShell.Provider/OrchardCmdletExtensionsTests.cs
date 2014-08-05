namespace Proligence.PowerShell.Provider.Tests
{
    using System;
    using System.Management.Automation;
    using Moq;
    using NUnit.Framework;
    using Proligence.PowerShell.Provider;

    [TestFixture]
    public class OrchardCmdletExtensionsTests
    {
        [Test]
        public void WriteErrorWhenCmdletNull()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            var exception = Assert.Throws<ArgumentNullException>(
                () => OrchardCmdletExtensions.WriteError(null, ex, "ErrorId", ErrorCategory.InvalidOperation, target));

            Assert.That(exception.ParamName, Is.EqualTo("cmdlet"));
        }

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

        [Test]
        public void ThrowTerminatingErrorWhenCmdletNull()
        {
            var ex = new InvalidOperationException("My exception message.");
            var target = new object();

            var exception = Assert.Throws<ArgumentNullException>(
                () => OrchardCmdletExtensions.ThrowTerminatingError(
                    null, ex, "ErrorId", ErrorCategory.InvalidOperation, target));

            Assert.That(exception.ParamName, Is.EqualTo("cmdlet"));
        }

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