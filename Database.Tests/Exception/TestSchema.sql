CREATE SCHEMA [TestSchema];
GO

CREATE PROCEDURE [TestSchema].[test that messed up transaction raise an exception]
AS
BEGIN
    BEGIN TRANSACTION

    EXEC tSQLt.AssertEquals 'hallo', 'hallo';

    ROLLBACK TRANSACTION
END;
GO