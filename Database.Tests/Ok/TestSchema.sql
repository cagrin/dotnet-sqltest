CREATE SCHEMA [TestSchema];
GO

CREATE PROCEDURE [TestSchema].[test that this test is ok]
AS
BEGIN
    EXEC dbo.Assertions;
END;
GO

CREATE PROCEDURE [dbo].[Assertions]
AS
BEGIN
    EXEC tSQLt.AssertEquals 'hallo', 'hallo';

    EXEC tSQLt.AssertEqualsString 'hallo', 'hallo';

    EXEC tSQLt.AssertNotEquals 'hello', 'hallo';
END;
GO

CREATE PROCEDURE [dbo].[Example]
AS
BEGIN
    SELECT A = 1 INTO #Example

    UPDATE #Example SET
        A = 2
END;
GO