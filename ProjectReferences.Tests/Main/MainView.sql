CREATE VIEW dbo.MainView
AS
    SELECT
        OtherId,
        OtherColumn
    FROM OtherDatabase.dbo.OtherTable
GO