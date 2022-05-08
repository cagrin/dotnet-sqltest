CREATE SCHEMA [TestSchema];
GO

CREATE PROCEDURE [TestSchema].[test that database has tha same collation than server]
AS
BEGIN
    SELECT collation_name = 'Polish_CI_AS'
    INTO #Expected;

    SELECT DISTINCT collation_name
    INTO #Actual FROM sys.databases;

    EXEC tSQLt.AssertEqualsTable '#Expected', '#Actual';
END;
GO