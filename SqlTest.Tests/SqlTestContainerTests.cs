namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class SqlTestContainerTests
{
    [TestMethod]
    public void ShoudRunDispose()
    {
        using var stc = new SqlTestContainer();
    }
}