namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MainShouldThrowArgumentNullException()
    {
        Program.Main(null!);
    }

    [TestMethod]
    public void MainShouldRunWithEmptyArgs()
    {
        Program.Main(Array.Empty<string>());
    }
}