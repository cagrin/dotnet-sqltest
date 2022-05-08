CREATE SCHEMA [TestSchema];
GO

CREATE PROCEDURE [TestSchema].[test that this test fails]
AS
BEGIN
    EXEC tSQLt.AssertEquals 'hallo2', 'hallo';
END;
GO