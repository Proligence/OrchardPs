namespace Orchard.Tests.PowerShell.Infrastructure
{
    using Xunit;

    public static class TestConsoleConnectionExtensions
    {
        public static void AssertNoErrors(this TestConsoleConnection connection)
        {
            string error = connection.ErrorOutput.ToString();
            if (!string.IsNullOrEmpty(error))
            {
                Assert.True(false, error);
            }
        }
    }
}