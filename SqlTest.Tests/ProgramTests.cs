namespace SqlTest.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MainShouldThrowArgumentNullException()
    {
        SqlTest.Program.Main(null!);
    }

    [TestMethod]
    public void MainShouldRunWithEmptyArgs()
    {
        SqlTest.Program.Main(Array.Empty<string>());
    }
}