namespace SqlTest.Tests;

using SqlTest;

[TestClass]
public class TestcontainerFactoryTests
{
    [TestMethod]
    public void ShouldThrowArgumentNullException()
    {
        var exception = Assert.ThrowsException<ArgumentNullException>(() => _ = TestcontainerFactory.Create(null!));

        var message = "Value cannot be null. (Parameter 'image')";

        Assert.AreEqual(message, exception.Message);
    }

    [TestMethod]
    public void ShouldCreateMsSqlTestcontainer()
    {
        var testcontainer = TestcontainerFactory.Create("mcr.microsoft.com/mssql/server");

        Assert.IsInstanceOfType(testcontainer, typeof(MsSqlTestcontainer));
    }

    [TestMethod]
    public void ShouldCreateSqlEdgeTestcontainer()
    {
        var testcontainer = TestcontainerFactory.Create("mcr.microsoft.com/azure-sql-edge");

        Assert.IsInstanceOfType(testcontainer, typeof(SqlEdgeTestcontainer));
    }
}