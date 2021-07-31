using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mobile.BuildTools.Utils;
using Xunit;

namespace Mobile.BuildTools.Tests.Fixtures.Utils
{
    public class CodeWriterFixture
    {
        [Fact]
        public void ClosesBrackets()
        {
            var writer = new CodeWriter();

            writer.Block("namespace AwesomeApp");
            writer.Block("public class Foo");
            using (writer.Block("public Foo()"))
            {
                writer.AppendLine("var test = string.Empty;");
            }

            var expected = @"namespace AwesomeApp
{
    public class Foo
    {
        public Foo()
        {
            var test = string.Empty;
        }
    }
}
";

            var output = writer.ToString();
            Assert.Equal(expected, output, ignoreLineEndingDifferences: true);
        }

        [Fact]
        public void SafeOutputNotClosedWithoutToString()
        {
            var writer = new CodeWriter();

            writer.Block("namespace AwesomeApp");
            writer.Block("public class Foo");
            using (writer.Block("public Foo()"))
            {
                writer.AppendLine("var test = string.Empty;");
            }

            var expected = @"namespace AwesomeApp
{
    public class Foo
    {
        public Foo()
        {
            var test = string.Empty;
        }
";

            var output = writer.SafeOutput;
            Assert.Equal(expected, output, ignoreLineEndingDifferences: true);
        }

        [Fact]
        public void MaintainsSafeOutput()
        {
            var writer = new CodeWriter();

            writer.Block("namespace AwesomeApp");
            writer.Block("public class Foo");
            using (writer.Block("public Foo()"))
            {
                writer.AppendLine("var test = string.Empty;", "var test = ******;");
            }

            var expected = @"namespace AwesomeApp
{
    public class Foo
    {
        public Foo()
        {
            var test = string.Empty;
        }
    }
}
";
            var expectedSafe = @"namespace AwesomeApp
{
    public class Foo
    {
        public Foo()
        {
            var test = ******;
        }
    }
}
";

            var output = writer.ToString();
            var safe = writer.SafeOutput;
            Assert.Equal(expected, output, ignoreLineEndingDifferences: true);
            Assert.Equal(expectedSafe, safe, ignoreLineEndingDifferences: true);
        }
    }
}
