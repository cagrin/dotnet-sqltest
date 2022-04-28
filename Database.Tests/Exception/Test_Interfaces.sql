CREATE SCHEMA [Test_Interfaces];
GO

CREATE PROCEDURE [Test_Interfaces].[Test_Assertions]
AS
BEGIN
    BEGIN TRANSACTION

    EXEC tSQLt.AssertEquals 'hallo', 'hallo';

    ROLLBACK TRANSACTION
END;
GO