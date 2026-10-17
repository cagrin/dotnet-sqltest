namespace SqlTest.UnitTests;

[TestClass]
public class AdapterTests
{
    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(5, 3, 60)]
    [DataRow(3, 3, 100)]
    [DataRow(8, 1, 12)]
    public void ShouldCalculateCoveragePercent(long statements, long covered, long expected)
    {
        var sut = new CoverageReport(statements, covered, [], string.Empty);

        Assert.AreEqual(expected, sut.Percent);
    }

    [TestMethod]
    [DataRow("Database.Tests", "[Database.Tests]")]
    [DataRow("evil]; DROP DATABASE x; --", "[evil]]; DROP DATABASE x; --]")]
    public void ShouldQuoteName(string name, string expected)
    {
        Assert.AreEqual(expected, SqlServerClient.QuoteName(name));
    }

    [TestMethod]
    public void ShouldThrowWhenCoverageIsStoppedBeforeStart()
    {
        _ = Assert.Throws<InvalidOperationException>(() => new SqlCoverCodeCoverage().Stop());
    }

    [TestMethod]
    public async Task ShouldPingExplicitTarget()
    {
        var replay = new ReplayScenario(new Scenario());
        await using var sut = new ExplicitTarget("Server=myserver,1500;User ID=u;Password=p;Initial Catalog=Db", replay);

        var target = await sut.StartAsync(CancellationToken.None).ConfigureAwait(false);

        Assert.AreEqual("myserver", target.TargetServerName);
        Assert.AreEqual(1500, target.TargetPort);
        Assert.AreEqual($"Ping {target.TargetConnectionString}", replay.SqlCalls.Single());
    }

    [TestMethod]
    public void ShouldCreateSqlTestException()
    {
        var inner = new InvalidOperationException();

        Assert.IsNotNull(new SqlTestException().Message);
        Assert.AreEqual("message", new SqlTestException("message").Message);
        Assert.AreSame(inner, new SqlTestException("message", inner).InnerException);
    }
}
