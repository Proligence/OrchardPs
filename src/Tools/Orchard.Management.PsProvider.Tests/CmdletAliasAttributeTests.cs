// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CmdletAliasAttributeTests.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Tests
{
    using System;
    using NUnit.Framework;

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
        [TestCase(null)]
        [TestCase("")]
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
