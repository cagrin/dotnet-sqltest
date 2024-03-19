CREATE SCHEMA [TestMainFunction];
GO

CREATE PROCEDURE [TestMainFunction].[test that it can reference other databases by DatabaseVariableLiteralValue and DatabaseSqlCmdVariable]
AS
BEGIN
    SELECT
        MainId = 1,
        MainColumn = 'Value1'
    INTO #Expected
    UNION ALL
    SELECT
        MainId = 2,
        MainColumn = 'Value2'

    INSERT INTO OtherDatabase.dbo.OtherTable
    (
        OtherId,
        OtherColumn
    )
    SELECT
        MainId,
        MainColumn
    FROM #Expected
    WHERE MainId = 1

    INSERT INTO [$(SecondDatabase)].dbo.SecondTable
    (
        SecondId,
        SecondColumn
    )
    SELECT
        MainId,
        MainColumn
    FROM #Expected
    WHERE MainId = 2

    SELECT
        MainId,
        MainColumn
    INTO #Actual
    FROM dbo.MainFunction()

    EXEC tSQLt.AssertEqualsTable '#Expected', '#Actual'
END;
GO