namespace Proligence.PowerShell.Provider.Tests
{
    using System;
    using NUnit.Framework;
    using Proligence.PowerShell.Provider;

    [TestFixture]
    public class CmdletAliasAttributeTests
    {
        [TestCase("")]
        [TestCase(null)]
        public void TestConstructorWhenAliasNullOrEmpty(string alias)
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new CmdletAliasAttribute(alias));

            Assert.That(exception.ParamName, Is.EqualTo("alias"));
        }

        /// <summary>
        /// Tests if the constructor of the <see cref="CmdletAliasAttribute"/> class properly initializes a new
        /// instance when the specified constructor arguments are valid.
        /// </summary>
        [Test]
        public void TestConstructorWhenValidArgs()
        {
            var attr = new CmdletAliasAttribute("MyAlias");

            Assert.That(attr.Alias, Is.EqualTo("MyAlias"));
        }
    }
}
