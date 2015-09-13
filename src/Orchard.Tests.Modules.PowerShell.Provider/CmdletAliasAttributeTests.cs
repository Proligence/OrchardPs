using System;
using Proligence.PowerShell.Provider;
using Xunit;

namespace Orchard.Tests.Modules.PowerShell.Provider {
    public class CmdletAliasAttributeTests {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void TestConstructorWhenAliasNullOrEmpty(string alias) {
            var exception = Assert.Throws<ArgumentException>(
                () => new CmdletAliasAttribute(alias));

            Assert.Equal("alias", exception.ParamName);
        }

        [Fact]
        public void TestConstructorWhenValidArgs() {
            var attr = new CmdletAliasAttribute("MyAlias");

            Assert.Equal("MyAlias", attr.Alias);
        }
    }
}