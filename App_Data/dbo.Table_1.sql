CREATE TABLE [dbo].[Country] (
    [CountryID]   INT           IDENTITY (1, 1) NOT NULL,
    [CountryName] VARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([CountryID] ASC)
);
