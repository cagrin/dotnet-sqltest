CREATE SCHEMA [Test_Interfaces];
GO

CREATE PROCEDURE [Test_Interfaces2].[Test_Assertions]
AS
BEGIN
    EXEC tSQLt.AssertEquals 'hallo', 'hallo';
END;
GO