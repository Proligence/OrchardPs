namespace Orchard.Tests.Modules.PowerShell.Provider.Utilities
{
    using System.Collections;
    using Proligence.PowerShell.Provider.Utilities;
    using Xunit;

    public class ArgumentListTests
    {
        [Fact]
        public void ParseEmptyList()
        {
            var args = ArgumentList.Parse(new ArrayList());

            Assert.Empty(args);
        }

        [Fact]
        public void ParseSingleArgument()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo", "value1" });

            Assert.Equal(1, args.Count);
            Assert.Equal("value1", args["foo"]);
        }

        [Fact]
        public void ParseMultipleArguments()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo", "value1", "-bar", "value2", "-baz", "value3" });

            Assert.Equal(3, args.Count);
            Assert.Equal("value1", args["foo"]);
            Assert.Equal("value2", args["bar"]);
            Assert.Equal("value3", args["baz"]);
        }

        [Fact]
        public void ParseSwitch()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo" });

            Assert.Equal(1, args.Count);
            Assert.Null(args["foo"]);
        }

        [Fact]
        public void ParseSwitchWithArguments()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo", "value1", "-bar", "-baz", "value3" });

            Assert.Equal(3, args.Count);
            Assert.Equal("value1", args["foo"]);
            Assert.Null(args["bar"]);
            Assert.Equal("value3", args["baz"]);
        }

        [Fact]
        public void ParseArgumentWithNullValue()
        {
            var args = ArgumentList.Parse(new ArrayList { "-foo", "value1", "-bar", null, "-baz", "value3" });

            Assert.Equal(3, args.Count);
            Assert.Equal("value1", args["foo"]);
            Assert.Null(args["bar"]);
            Assert.Equal("value3", args["baz"]);
        }
    }
}