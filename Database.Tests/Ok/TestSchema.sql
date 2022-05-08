CREATE SCHEMA [TestSchema];
GO

CREATE PROCEDURE [TestSchema].[test that this test is ok]
AS
BEGIN
    EXEC dbo.Some_Assertions;
END;
GO

CREATE PROCEDURE [dbo].[Some_Assertions]
AS
BEGIN
    EXEC tSQLt.AssertEquals 'hallo', 'hallo';

    EXEC tSQLt.AssertEqualsString 'hallo', 'hallo';

    EXEC tSQLt.AssertNotEquals 'hello', 'hallo';
END;
GO

CREATE PROCEDURE [dbo].[Some_Uncovered]
AS
BEGIN
    EXEC tSQLt.AssertEquals 'uncovered', 'uncovered';

    EXEC tSQLt.AssertEqualsString 'uncovered', 'uncovered';
END;
GO