namespace SqlTest.Tests.UnitTests;

using SqlTest;

[TestClass]
public class ResultLogTests
{
    [TestMethod]
    public void ShouldGetFirstLineOnSingleLine()
    {
        string expression = @"Some single line";

        string expected = expression;

        string actual = expression.FirstLine();

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ShouldGetFirstLineOnMultipleLines()
    {
        string expression = @"    Some first line
        and second line
        and third.";

        string expected = @"    Some first line [...]";

        string actual = expression.FirstLine();

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ShouldGetFirstLineOnMultipleLinesWithFirstEmpty()
    {
        string expression = @"
        starts from second line
        and continue on third.";

        string expected = @" [...]";

        string actual = expression.FirstLine();

        Assert.AreEqual(expected, actual);
    }
}