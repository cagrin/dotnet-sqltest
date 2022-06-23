namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void MainShouldThrowArgumentNullException()
    {
        var exception = Assert.ThrowsException<ArgumentNullException>(() => _ = Program.Main(null!));

        var message = "Value cannot be null. (Parameter 'args')";

        Assert.AreEqual(message, exception.Message);
    }
}