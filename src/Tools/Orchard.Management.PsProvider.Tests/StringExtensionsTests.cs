namespace Orchard.Management.PsProvider.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void WildcardEqualsBothNull()
        {
            Assert.That(StringExtensions.WildcardEquals(null, null), Is.True);
        }

        [Test]
        public void WildcardEqualsStrNull()
        {
            Assert.That(StringExtensions.WildcardEquals(null, "foo"), Is.False);
        }

        [Test]
        public void WildcardEqualsPatternNull()
        {
            Assert.That("foo".WildcardEquals(null), Is.False);
        }

        [Test]
        public void WildcardEqualsStarInPattern()
        {
            Assert.That("foobar".WildcardEquals("f*bar"), Is.True);
            Assert.That("foobar".WildcardEquals("fa*bar"), Is.False);
        }

        [Test]
        public void WildcardEqualsMultipleStarsInPattern()
        {
            Assert.That("foobar".WildcardEquals("f*b*r"), Is.True);
            Assert.That("foobar".WildcardEquals("fa*b*r"), Is.False);
        }

        [Test]
        public void WildcardEqualsQuestionMarkInPattern()
        {
            Assert.That("foobar".WildcardEquals("foo?ar"), Is.True);
            Assert.That("foobar".WildcardEquals("f?bar"), Is.False);
        }

        [Test]
        public void WildcardEqualsSameString()
        {
            Assert.That("foobar".WildcardEquals("foobar"), Is.True);
        }

        [Test]
        public void WildcardEqualsSameStringButDifferentCase()
        {
            Assert.That("foobar".WildcardEquals("FooBar"), Is.True);
            Assert.That("foobar".WildcardEquals("FooBar", true), Is.False);
        }
    }
}