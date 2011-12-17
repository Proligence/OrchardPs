using System;
using NUnit.Framework;

namespace Orchard.Management.PsProvider.Tests {
    [TestFixture]
    public class OrchardProviderExceptionTests {
        [Test]
        public void TestCtor() {
            var exception = new OrchardProviderException();
            Assert.AreEqual("Exception of type 'Orchard.Management.PsProvider.OrchardProviderException' was thrown.", 
                            exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public void TestCtorWithMessage() {
            var exception = new OrchardProviderException("My message.");
            Assert.AreEqual("My message.", exception.Message);
            Assert.IsNull(exception.InnerException);
        }

        [Test]
        public void TestCtorWithMessageAndInnerException() {
            var inner = new Exception();
            var exception = new OrchardProviderException("My message.", inner);
            Assert.AreEqual("My message.", exception.Message);
            Assert.AreSame(inner, exception.InnerException);
        }
    }
}