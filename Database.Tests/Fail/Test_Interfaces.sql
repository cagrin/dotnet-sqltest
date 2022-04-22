CREATE SCHEMA [Test_Interfaces];
GO

CREATE PROCEDURE [Test_Interfaces].[Test_Assertions]
AS
BEGIN
    EXEC tSQLt.AssertEquals 'hallo2', 'hallo';
END;
GO