CREATE SCHEMA [TestSchema];
GO

CREATE PROCEDURE [TestSchema].[test that this procedure raise an build error]
AS
BEGIN
    [Make this an build error].

    EXEC tSQLt.AssertEquals 'hallo', 'hallo';
END;
GO