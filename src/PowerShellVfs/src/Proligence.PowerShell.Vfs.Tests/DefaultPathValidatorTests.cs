// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultPathValidatorTests.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Tests
{
    using NUnit.Framework;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements unit tests for the <see cref="DefaultPathValidator"/> class.
    /// </summary>
    [TestFixture]
    public class DefaultPathValidatorTests
    {
        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsValidPath"/> method returns <c>false</c> if the specified
        /// path is <c>null</c> or empty.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase(null)]
        [TestCase("")]
        public void IsValidPathWhenPathNullOrEmpty(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsValidPath(path);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsValidPath"/> method returns <c>true</c> if the specified
        /// path is a root path.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase(@"Root:\")]
        [TestCase(@"Root:")]
        public void IsValidPathWhenRootPath(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsValidPath(path);

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsValidPath"/> method returns <c>true</c> if the specified
        /// path is a rooted path.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase(@"Root:\My\Path")]
        [TestCase(@"Root:\My\Path\")]
        public void IsValidPathWhenRootedPath(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsValidPath(path);

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsValidPath"/> method returns <c>true</c> if the specified
        /// path is a relative path.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase(@"My\Path\")]
        [TestCase(@"My\Path")]
        public void IsValidPathWhenRelativePath(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsValidPath(path);

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsValidPath"/> method returns <c>false</c> if the specified
        /// path is a root path with an empty drive name.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase(@":\")]
        [TestCase(@":")]
        public void IsValidPathWhenEmptyDriveName(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsValidPath(path);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsValidPath"/> method returns <c>false</c> if the specified
        /// path begins with a slash.
        /// </summary>
        [Test]
        public void IsValidPathWhenPathBeginsWithSlash()
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsValidPath(@"\My\Path");

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsValidPath"/> method returns <c>false</c> if one of the
        /// directories in the specified path is empty.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase(@"Root:\\")]
        [TestCase(@"Root:\My\\Path")]
        [TestCase(@"Root:\My\Path\\")]
        public void IsValidPathWhenPathContainsEmptyDirectoryName(string path)
        {
            var validator = new DefaultPathValidator(); 

            bool result = validator.IsValidPath(path);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsValidPath"/> method returns <c>false</c> if one of the
        /// directories in the specified path contains an invalid (unallowed) character.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase(@"Root:\M:y\Path")]
        [TestCase(@"Roo:t:\My\Path")]
        public void IsValidPathWhenPathContainsInvalidChars(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsValidPath(path);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsDrivePath"/> method returns <c>false</c> if the specified
        /// path is <c>null</c> or empty.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase(null)]
        [TestCase("")]
        public void IsDrivePathWhenPathNullOrEmpty(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsDrivePath(path, "VFS:\\");

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsDrivePath"/> method returns <c>false</c> if the specified
        /// root path is <c>null</c> or empty.
        /// </summary>
        /// <param name="path">The tested root path.</param>
        [TestCase(null)]
        [TestCase("")]
        public void IsDrivePathWhenRootPathNullOrEmpty(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsDrivePath("VFS:\\", path);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsDrivePath"/> method returns <c>true</c> if the specified
        /// path is a drive path.
        /// </summary>
        [Test]
        public void IsDrivePathWhenDrivePathSpecified()
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsDrivePath("VFS:\\", "VFS:\\");

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsDrivePath"/> method returns <c>false</c> if the specified
        /// path is a not a drive path.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase("VFS:\\Dir")]
        [TestCase("VFS:\\Dir\\Subdir")]
        [TestCase("VFS:\\Dir\\Subdir\\")]
        public void IsDrivePathWhenNonDrivePathSpecified(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsDrivePath("VFS:\\", path);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsDrivePath"/> method returns <c>false</c> if the specified
        /// path is a relative path.
        /// </summary>
        /// <param name="path">The tested path.</param>
        [TestCase("\\Dir")]
        [TestCase("\\Dir\\Subdir")]
        [TestCase("Dir")]
        [TestCase("Dir\\Subdir")]
        [TestCase("Dir\\")]
        [TestCase("Dir\\Subdir\\")]
        public void IsDrivePathWhenRelativePathSpecified(string path)
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsDrivePath("VFS:\\", path);

            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.IsDrivePath"/> method compares paths using case-insensitive
        /// comparison.
        /// </summary>
        [Test]
        public void IsDrivePathWhenSpecifiedPathInOtherCase()
        {
            var validator = new DefaultPathValidator();

            bool result = validator.IsDrivePath("Vfs:\\", "VFS:\\");

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.JoinPath"/> method correctly joins null paths.
        /// </summary>
        /// <param name="left">Left path</param>
        /// <param name="right">Right path</param>
        /// <param name="expected">Expected result.</param>
        [TestCase(@"a\b", null, @"a\b")]
        [TestCase(null, @"a\b", @"a\b")]
        [TestCase(null, null, @"")]
        public void JoinPathWhenPathNull(string left, string right, string expected)
        {
            var validator = new DefaultPathValidator();

            string result = validator.JoinPath(left, right);

            Assert.That(result, Is.EqualTo(expected));
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.JoinPath"/> correctly joins two paths.
        /// </summary>
        /// <param name="left">Left path</param>
        /// <param name="right">Right path</param>
        /// <param name="expected">Expected result.</param>
        [TestCase(@"a\b", @"c\d", @"a\b\c\d")]
        public void JoinPathWhenCommonCase(string left, string right, string expected)
        {
            var validator = new DefaultPathValidator();

            string result = validator.JoinPath(left, right);

            Assert.That(result, Is.EqualTo(expected));
        }

        /// <summary>
        /// Tests if the <see cref="DefaultPathValidator.JoinPath"/> correctly joins two paths which begin or end with
        /// the path separator symbol.
        /// </summary>
        /// <param name="left">Left path</param>
        /// <param name="right">Right path</param>
        /// <param name="expected">Expected result.</param>
        [TestCase(@"a\b\", @"c\d", @"a\b\c\d")]
        [TestCase(@"a\b", @"\c\d", @"a\b\c\d")]
        [TestCase(@"a\b\", @"\c\d", @"a\b\c\d")]
        public void JoinPathWhenPathBeginsOrEndsWithSeparator(string left, string right, string expected)
        {
            var validator = new DefaultPathValidator();

            string result = validator.JoinPath(left, right);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}