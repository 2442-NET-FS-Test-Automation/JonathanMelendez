USE LibraryDB;
GO

/*
DROP TABLE dbo.Loan;
DROP TABLE dbo.Member;
DROP TABLE dbo.Book;
DROP TABLE dbo.Author;
*/

-- =====   Section 1 - DDL   =====

-- Create tables
CREATE TABLE dbo.Author(
    AuthorId        int             IDENTITY(1,1),
    FirstName       varchar(50)     NOT NULL,
    LastName        varchar(50)     NOT NULL,
    BirthYear       int             NULL,

    CONSTRAINT PK_Author PRIMARY KEY(AuthorId),
    CONSTRAINT CK_Author_BirthYear CHECK (BirthYear IS NULL OR BirthYear BETWEEN 300 AND 2100)
);

CREATE TABLE dbo.Book(
    BookId          int             IDENTITY(1,1),
    Title           varchar(200)    NOT NULL,
    ISBN            char(13)        NOT NULL,
    PublishedYear   int             NULL,
    CategoryName    varchar(60)     NOT NULL            CONSTRAINT DF_Book_CategoryName DEFAULT('General'),
    AuthorId        int             NOT NULL,
    TotalCopies     int             NOT NULL            CONSTRAINT DF_Book_TotalCopies DEFAULT(1),
    AvailableCopies int             NOT NULL            CONSTRAINT DF_Book_AvailableCopies DEFAULT(1),

    CONSTRAINT UQ_Book_ISBN     UNIQUE(ISBN),

    CONSTRAINT PK_Book          PRIMARY KEY(BookId),
    CONSTRAINT FK_Book_Author   FOREIGN KEY(AuthorId) REFERENCES dbo.Author(AuthorId) ON DELETE CASCADE,
    CONSTRAINT CK_Book_Copies   CHECK(TotalCopies >= AvailableCopies),
);

CREATE TABLE dbo.Member(
    MemberId        int             IDENTITY(1,1),
    FirstName       varchar(50)     NOT NULL,
    LastName        varchar(50)     NOT NULL,
    Email           varchar(125)    UNIQUE NOT NULL,
    JoinedDate      date            NOT NULL DEFAULT (GETDATE()),

    CONSTRAINT PK_Member PRIMARY KEY(MemberId),
);

CREATE TABLE dbo.Loan(
    LoanId          int             IDENTITY(1,1),
    BookId          int             NOT NULL,
    MemberId        int             NOT NULL,
    LoanDate        date            NOT NULL            CONSTRAINT DF_Loan_LoanDate DEFAULT(GETDATE()),
    DueDate         date            NOT NULL,
    ReturnDate      date            NULL,

    CONSTRAINT PK_Loan          PRIMARY KEY(LoanId),
    CONSTRAINT FK_Loan_Book     FOREIGN KEY(BookId) REFERENCES dbo.Book(BookId),
    CONSTRAINT FK_Loan_Member   FOREIGN KEY(MemberId) REFERENCES dbo.Member(MemberId),
    CONSTRAINT CK_Loan_Dates    CHECK(DueDate >= LoanDate)
);

GO

-- Alter tables
ALTER TABLE dbo.Book ADD
    Edition int NOT NULL CONSTRAINT DF_Book_Edition DEFAULT(1);

ALTER TABLE dbo.Book ALTER COLUMN 
    Title varchar(300) NOT NULL;

GO

-- =====   Section 2 - DML + DQL (CRUD)   =====

INSERT INTO dbo.Author (FirstName, LastName, BirthYear) VALUES
    ('Robert',  'Martin',   1952),   -- 1
    ('Martin',  'Fowler',   1963),   -- 2
    ('Kent',    'Beck',     1961),   -- 3
    ('Erich',   'Gamma',    1961),   -- 4
    ('Andrew',  'Hunt',     1964),   -- 5
    ('David',   'Thomas',   1956);   -- 6
GO

INSERT INTO dbo.Member (FirstName, LastName, Email, JoinedDate) VALUES
    ('Ada',     'Lovelace', 'ada@example.com',     '2025-01-10'),  -- 1
    ('Grace',   'Hopper',   'grace@example.com',   '2025-02-02'),  -- 2
    ('Alan',    'Turing',   'alan@example.com',    '2025-02-20'),  -- 3
    ('Linus',   'Torvalds', 'linus@example.com',   '2025-03-15'),  -- 4
    ('Margaret','Hamilton', 'margaret@example.com','2025-04-01'),  -- 5
    ('Dennis',  'Ritchie',  'dennis@example.com',  '2025-05-05');  -- 6
GO

INSERT INTO dbo.Book (Title, ISBN, PublishedYear, CategoryName, AuthorId, TotalCopies, AvailableCopies, Edition) VALUES
    ('Clean Code',                         '9780132350884', 2008, 'Software',               1, 3, 3, 1),
    ('Clean Architecture',                 '9780134494166', 2017, 'Software',               1, 2, 2, 1),
    ('Refactoring',                        '9780201485677', 1999, 'Software',               2, 2, 1, 2),
    ('Patterns of Enterprise Application Architecture','9780321127426',2002,'Software',     2, 1, 1, 1),
    ('Test Driven Development',            '9780321146533', 2002, 'Testing',                3, 2, 2, 1),
    ('Extreme Programming Explained',      '9780321278654', 2004, 'Process',                3, 1, 0, 2),
    ('Design Patterns',                    '9780201633610', 1994, 'Software',               4, 2, 2, 1),
    ('The Pragmatic Programmer',           '9780201616224', 1999, 'Software',               5, 4, 3, 1),
    ('The Pragmatic Programmer 20th Anniv','9780135957059', 2019, 'Software',               5, 2, 2, 2),
    ('Programming Ruby',                   '9780974514055', 2004, 'Languages',              6, 1, 1, 1);
GO

INSERT INTO dbo.Loan(BookId, MemberId, DueDate) VALUES
    (6, 1, '2026-06-30');

GO

UPDATE dbo.Book SET Edition = 2 WHERE BookId = 3;

UPDATE dbo.Book SET AvailableCopies = AvailableCopies-1 WHERE BookId = 1;

-- DELETE FROM dbo.Member WHERE Email = 'margaret@example.com';

-- Every Title post 2000
SELECT Title, PublishedYear FROM dbo.Book WHERE PublishedYear >= 2000;

-- Every Title from 1999 to 2004
SELECT Title from dbo.Book WHERE PublishedYear BETWEEN 1999 AND 2004;

-- Filter by a list of options (By default are case-insensitive for comparisions)
SELECT Title, CategoryName FROM dbo.Book WHERE CategoryName IN ('Software', 'testing');

-- pattern matching
SELECT Title FROM dbo.Book WHERE Title LIKE 'Test%';

-- Multiple comparisions
SELECT Title FROM dbo.Book WHERE CategoryName = 'Software' AND AvailableCopies > 1;

-- Null fields
SELECT Title FROM dbo.Book WHERE PublishedYear IS NULL;

-- ORDER and DISTINCT
-- Order by default is asc, can use multiple orders
SELECT  Title, PublishedYear FROM dbo.Book ORDER BY PublishedYear DESC, Title ASC;

-- Doesnt show duplicate entries
SELECT DISTINCT CategoryName FROM dbo.Book ORDER BY CategoryName;

-- GROUP BY and HAVING
SELECT CategoryName, COUNT(*) AS BookCount FROM dbo.Book 
GROUP BY CategoryName HAVING COUNT(*) > 2 
ORDER BY BookCount DESC;