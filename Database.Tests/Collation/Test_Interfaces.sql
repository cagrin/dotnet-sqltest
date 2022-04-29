CREATE SCHEMA [Test_Interfaces];
GO

CREATE PROCEDURE [Test_Interfaces].[Test_Collations]
AS
BEGIN
    SELECT collation_name = 'Polish_CI_AS'
    INTO #Expected;

    SELECT DISTINCT collation_name
    INTO #Actual FROM sys.databases;

    EXEC tSQLt.AssertEqualsTable '#Expected', '#Actual';
END;
GO