namespace Orchard.Tests.Modules.PowerShell.Provider.Utilities
{
    using Proligence.PowerShell.Provider.Utilities;
    using Xunit;

    public class PropertyMapperTests
    {
        [Fact]
        public void MapPropertiesShouldMapProperties()
        {
            var foo = new Foo { A = "text", B = 5, C = true };
            var bar = new Bar();

            PropertyMapper.Instance.MapProperties(foo, bar);

            Assert.Equal("text", bar.A);
            Assert.Equal(5, bar.B);
            Assert.Equal(true, bar.C);
        }

        [Fact]
        public void MapPropertiesShouldNotMapPropertiesDisabledAtSource()
        {
            var foo = new Foo { D = "text1", E = "text2" };
            var bar = new Bar();

            PropertyMapper.Instance.MapProperties(foo, bar);

            Assert.Null(bar.D);
            Assert.Null(bar.E);
        }

        [Fact]
        public void MapPropertiesShouldNotMapPropertiesDisabledAtTarget()
        {
            var foo = new Foo { F = "text" };
            var bar = new Bar();

            PropertyMapper.Instance.MapProperties(foo, bar);

            Assert.Null(bar.F);
        }

        /* ReSharper disable UnusedAutoPropertyAccessor.Local */

        private class Foo
        {
            [Mappable]
            public string A { get; set; }

            [Mappable]
            public int B { get; set; }

            [Mappable]
            public bool C { get; set; }

            public string D { get; set; }

            [Mappable(false)]
            public string E { get; set; }

            [Mappable]
            public string F { get; set; }
        }

        private class Bar
        {
            public string A { get; set; }
            public int B { get; set; }
            public bool C { get; set; }
            public string D { get; set; }
            public string E { get; set; }

            [Mappable(false)]
            public string F { get; set; }
        }
    }
}