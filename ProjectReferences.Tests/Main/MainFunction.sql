CREATE FUNCTION dbo.MainFunction()
RETURNS @Result TABLE
(
    MainId INT NOT NULL,
    MainColumn VARCHAR(100) NOT NULL
)
AS
BEGIN
    INSERT INTO @Result
    SELECT
        OtherId,
        OtherColumn
    FROM OtherDatabase.dbo.OtherTable

    INSERT INTO @Result
    SELECT
        SecondId,
        SecondColumn
    FROM SecondDatabase.dbo.SecondTable

    RETURN
END