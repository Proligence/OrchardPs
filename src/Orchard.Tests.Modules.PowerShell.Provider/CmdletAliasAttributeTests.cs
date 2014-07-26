namespace Proligence.PowerShell.Provider.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using NUnit.Framework;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements unit tests for the <see cref="CmdletAliasAttribute"/> class.
    /// </summary>
    [TestFixture]
    public class CmdletAliasAttributeTests
    {
        /// <summary>
        /// Tests if the constructor of the <see cref="CmdletAliasAttribute"/> throws an
        /// <see cref="ArgumentNullException"/> when the specified alias is <c>null</c> or empty.
        /// </summary>
        /// <param name="alias">The tested alias.</param>
        [TestCase("")]
        [TestCase(null)]
        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults")]
        public void TestConstructorWhenAliasNullOrEmpty(string alias)
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
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
