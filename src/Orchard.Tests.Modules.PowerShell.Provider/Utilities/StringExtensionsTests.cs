using Proligence.PowerShell.Provider.Utilities;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Provider.Utilities {
    public class StringExtensionsTests {
        [Fact]
        public void WildcardEqualsBothNull() {
            Assert.True(StringExtensions.WildcardEquals(null, null));
        }

        [Fact]
        public void WildcardEqualsStrNull() {
            Assert.False(StringExtensions.WildcardEquals(null, "foo"));
        }

        [Fact]
        public void WildcardEqualsPatternNull() {
            Assert.False("foo".WildcardEquals(null));
        }

        [Fact]
        public void WildcardEqualsStarInPattern() {
            Assert.True("foobar".WildcardEquals("f*bar"));
            Assert.False("foobar".WildcardEquals("fa*bar"));
        }

        [Fact]
        public void WildcardEqualsMultipleStarsInPattern() {
            Assert.True("foobar".WildcardEquals("f*b*r"));
            Assert.False("foobar".WildcardEquals("fa*b*r"));
        }

        [Fact]
        public void WildcardEqualsQuestionMarkInPattern() {
            Assert.True("foobar".WildcardEquals("foo?ar"));
            Assert.False("foobar".WildcardEquals("f?bar"));
        }

        [Fact]
        public void WildcardEqualsSameString() {
            Assert.True("foobar".WildcardEquals("foobar"));
        }

        [Fact]
        public void WildcardEqualsSameStringButDifferentCase() {
            Assert.True("foobar".WildcardEquals("FooBar"));
            Assert.False("foobar".WildcardEquals("FooBar", true));
        }
    }
}