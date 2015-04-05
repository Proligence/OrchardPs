namespace Orchard.Tests.Modules.PowerShell.Provider
{
    using System;
    using Proligence.PowerShell.Provider;
    using Xunit;

    public class CmdletAliasAttributeTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void TestConstructorWhenAliasNullOrEmpty(string alias)
        {
            var exception = Assert.Throws<ArgumentException>(
                () => new CmdletAliasAttribute(alias));

            Assert.Equal("alias", exception.ParamName);
        }

        [Fact]
        public void TestConstructorWhenValidArgs()
        {
            var attr = new CmdletAliasAttribute("MyAlias");

            Assert.Equal("MyAlias", attr.Alias);
        }
    }
}