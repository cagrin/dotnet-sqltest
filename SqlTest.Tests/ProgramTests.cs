namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MainShouldThrowArgumentNullException()
    {
        _ = Program.Main(null!);
    }
}