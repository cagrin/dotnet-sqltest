CREATE SCHEMA [Test_Interfaces];
GO

CREATE PROCEDURE [Test_Interfaces].[Test_Assertions]
AS
BEGIN
    EXEC tSQLt.AssertEquals 'hallo', 'hallo';
    EXEC tSQLt.AssertEquals NULL, NULL;
    EXEC tSQLt.AssertEquals 5, 5;
    EXEC tSQLt.AssertEquals 3.14, 3.14;

    EXEC tSQLt.AssertEqualsString 'hallo', 'hallo';
    EXEC tSQLt.AssertEqualsString NULL, NULL;

    EXEC tSQLt.AssertNotEquals 'hello', 'hallo';
    EXEC tSQLt.AssertNotEquals 'hello', NULL;
    EXEC tSQLt.AssertNotEquals 5, 6;
    EXEC tSQLt.AssertNotEquals 3.14, 3.141;
END;
GO