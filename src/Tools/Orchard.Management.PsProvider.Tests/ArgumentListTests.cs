namespace Orchard.Management.PsProvider.Tests
{
    using System.Collections;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentListTests
    {
        [Test]
        public void ParseEmptyList()
        {
            var args = ArgumentList.Parse(new ArrayList());

            Assert.That(args, Is.Empty);
        }

        [Test]
        public void ParseSingleArgument()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo", "value1" });

            Assert.That(args.Count, Is.EqualTo(1));
            Assert.That(args["foo"], Is.EqualTo("value1"));
        }

        [Test]
        public void ParseMultipleArguments()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo", "value1", "-bar", "value2", "-baz", "value3" });

            Assert.That(args.Count, Is.EqualTo(3));
            Assert.That(args["foo"], Is.EqualTo("value1"));
            Assert.That(args["bar"], Is.EqualTo("value2"));
            Assert.That(args["baz"], Is.EqualTo("value3"));
        }

        [Test]
        public void ParseSwitch()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo" });

            Assert.That(args.Count, Is.EqualTo(1));
            Assert.That(args["foo"], Is.Null);
        }

        [Test]
        public void ParseSwitchWithArguments()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo", "value1", "-bar", "-baz", "value3" });

            Assert.That(args.Count, Is.EqualTo(3));
            Assert.That(args["foo"], Is.EqualTo("value1"));
            Assert.That(args["bar"], Is.Null);
            Assert.That(args["baz"], Is.EqualTo("value3"));
        }

        [Test]
        public void ParseArgumentWithNullValue()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo", "value1", "-bar", null, "-baz", "value3" });

            Assert.That(args.Count, Is.EqualTo(3));
            Assert.That(args["foo"], Is.EqualTo("value1"));
            Assert.That(args["bar"], Is.Null);
            Assert.That(args["baz"], Is.EqualTo("value3"));
        }
    }
}