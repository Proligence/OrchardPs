// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerShellVfsModuleTests.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Tests
{
    using Autofac;
    using Moq;
    using NUnit.Framework;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;
    using Proligence.PowerShell.Vfs.Provider;

    /// <summary>
    /// Implements unit tests for the <see cref="PowerShellVfsModule"/> class.
    /// </summary>
    [TestFixture]
    public class PowerShellVfsModuleTests
    {
        /// <summary>
        /// The tested container.
        /// </summary>
        private IContainer container;

        /// <summary>
        /// Prepares the test fixture before the tests are run.
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<PowerShellVfsModule>();
            builder.RegisterInstance(new Mock<VfsProvider>().Object).As<VfsProvider>();

            this.container = builder.Build();
        }

        /// <summary>
        /// Cleans up the test fixture after running all tests.
        /// </summary>
        [TestFixtureTearDown]
        public void Teardown()
        {
            if (this.container != null)
            {
                this.container.Dispose();
            }
        }

        /// <summary>
        /// Tests if the <see cref="IPowerShellConsole"/> interface is resolved.
        /// </summary>
        [Test]
        public void ResolvePowerShellConsole()
        {
            Assert.That(this.container.Resolve<IPowerShellConsole>(), Is.InstanceOf<PowerShellConsole>());
        }

        /// <summary>
        /// Tests if the <see cref="IPathValidator"/> interface is resolved.
        /// </summary>
        [Test]
        public void ResolvePathValidator()
        {
            Assert.That(this.container.Resolve<IPathValidator>(), Is.InstanceOf<DefaultPathValidator>());
        }

        /// <summary>
        /// Tests if the <see cref="INavigationProviderManager"/> interface is resolved.
        /// </summary>
        [Test]
        public void ResolveNavigationProviderManager()
        {
            var result = this.container.Resolve<INavigationProviderManager>(
                new NamedParameter("scope", null));
            
            Assert.That(result, Is.InstanceOf<NavigationProviderManager>());
        }
    }
}