CREATE SCHEMA [TestOtherDatabase];
GO

CREATE PROCEDURE [TestOtherDatabase].[test that it can reference other database by DatabaseVariableLiteralValue]
AS
BEGIN
    SELECT
        OtherId = 1,
        OtherColumn = 'OtherValue'
    INTO #Expected

    INSERT INTO OtherDatabase.dbo.OtherTable
    SELECT
        OtherId,
        OtherColumn
    FROM #Expected

    SELECT
        OtherId,
        OtherColumn
    INTO #Actual
    FROM dbo.MainView

    EXEC tSQLt.AssertEqualsTable '#Expected', '#Actual'
END;
GO