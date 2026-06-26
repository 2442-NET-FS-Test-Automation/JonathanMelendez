-- ---- DROP: children before parents ----------------------------------------------
DROP TABLE IF EXISTS dbo.Loan;
DROP TABLE IF EXISTS dbo.BookAuthor;
DROP TABLE IF EXISTS dbo.Book;
DROP TABLE IF EXISTS dbo.Member;
DROP TABLE IF EXISTS dbo.Author;
DROP TABLE IF EXISTS dbo.Category;
GO

-- ---- CREATE normalized: parents before children ---------------------------------
-- NEW: Category is now its own entity (was the free-text Book.CategoryName).
CREATE TABLE dbo.Category
(
    CategoryId  INT IDENTITY(1,1) NOT NULL,
    Name        VARCHAR(60)  NOT NULL,
    Description VARCHAR(200) NULL,
    CONSTRAINT PK_Category PRIMARY KEY (CategoryId),
    CONSTRAINT UQ_Category_Name UNIQUE (Name)
);

CREATE TABLE dbo.Author
(
    AuthorId  INT IDENTITY(1,1) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName  VARCHAR(50) NOT NULL,
    BirthYear INT NULL,
    CONSTRAINT PK_Author PRIMARY KEY (AuthorId),
    CONSTRAINT CK_Author_BirthYear CHECK (BirthYear IS NULL OR BirthYear BETWEEN 300 AND 2050)
);

CREATE TABLE dbo.Member
(
    MemberId   INT IDENTITY(1,1) NOT NULL,
    FirstName  VARCHAR(50)  NOT NULL,
    LastName   VARCHAR(50)  NOT NULL,
    Email      VARCHAR(125) NOT NULL,
    JoinedDate DATE NOT NULL CONSTRAINT DF_Member_JoinedDate DEFAULT (GETDATE()),
    CONSTRAINT PK_Member PRIMARY KEY (MemberId),
    CONSTRAINT UQ_Member_Email UNIQUE (Email)
);

-- CHANGED: Book now carries CategoryId (FK) instead of CategoryName, and has NO AuthorId
-- (authorship lives in the BookAuthor bridge). Edition and the wider Title are folded into
-- CREATE rather than added later with ALTER.
CREATE TABLE dbo.Book
(
    BookId          INT IDENTITY(1,1) NOT NULL,
    Title           VARCHAR(250) NOT NULL,
    ISBN            CHAR(13) NOT NULL,
    PublishedYear   INT NULL,
    CategoryId      INT NOT NULL,
    TotalCopies     INT NOT NULL CONSTRAINT DF_Book_TotalCopies     DEFAULT (1),
    AvailableCopies INT NOT NULL CONSTRAINT DF_Book_AvailableCopies DEFAULT (1),
    Edition         INT NOT NULL CONSTRAINT DF_Book_Edition         DEFAULT (1),
    CONSTRAINT PK_Book PRIMARY KEY (BookId),
    CONSTRAINT UQ_Book_ISBN UNIQUE (ISBN),
    CONSTRAINT CK_Book_Copies CHECK (TotalCopies >= AvailableCopies),
    CONSTRAINT FK_Book_Category FOREIGN KEY (CategoryId) REFERENCES dbo.Category (CategoryId)
);

-- NEW: the M:N bridge between Book and Author. Composite PK, no extra attributes.
CREATE TABLE dbo.BookAuthor
(
    BookId   INT NOT NULL,
    AuthorId INT NOT NULL,
    CONSTRAINT PK_BookAuthor PRIMARY KEY (BookId, AuthorId),
    CONSTRAINT FK_BookAuthor_Book   FOREIGN KEY (BookId)   REFERENCES dbo.Book   (BookId)   ON DELETE CASCADE,
    CONSTRAINT FK_BookAuthor_Author FOREIGN KEY (AuthorId) REFERENCES dbo.Author (AuthorId) ON DELETE CASCADE
);

CREATE TABLE dbo.Loan
(
    LoanId     INT IDENTITY(1,1) NOT NULL,
    BookId     INT NOT NULL,
    MemberId   INT NOT NULL,
    LoanDate   DATE NOT NULL CONSTRAINT DF_Loan_LoanDate DEFAULT (GETDATE()),
    DueDate    DATE NOT NULL,
    ReturnDate DATE NULL,
    CONSTRAINT PK_Loan PRIMARY KEY (LoanId),
    CONSTRAINT FK_Loan_Book   FOREIGN KEY (BookId)   REFERENCES dbo.Book   (BookId),
    CONSTRAINT FK_Loan_Member FOREIGN KEY (MemberId) REFERENCES dbo.Member (MemberId),
    CONSTRAINT CK_Loan_Dates  CHECK (DueDate >= LoanDate)
);
GO


-- ---- SEED normalized: parents before children -----------------------------------
-- Category rows are seeded directly (no SELECT DISTINCT off Book, since Book no longer
-- holds the name). Comments mark the IDENTITY value each row receives.
INSERT INTO dbo.Category (Name, Description) VALUES
    ('Software',  'Software design and craftsmanship'),  -- 1
    ('Testing',   'Testing and TDD'),                    -- 2
    ('Process',   'Process and methodology'),            -- 3
    ('Languages', 'Programming languages');              -- 4

INSERT INTO dbo.Author (FirstName, LastName, BirthYear) VALUES
    ('Robert', 'Martin', 1952),   -- 1
    ('Martin', 'Fowler', 1963),   -- 2
    ('Kent',   'Beck',   1961),   -- 3
    ('Erich',  'Gamma',  1961),   -- 4
    ('Andrew', 'Hunt',   1964),   -- 5
    ('David',  'Thomas', 1956);   -- 6

INSERT INTO dbo.Member (FirstName, LastName, Email, JoinedDate) VALUES
    ('Ada',     'Lovelace', 'ada@example.com',      '2025-01-10'),  -- 1
    ('Grace',   'Hopper',   'grace@example.com',    '2025-02-02'),  -- 2
    ('Alan',    'Turing',   'alan@example.com',     '2025-02-20'),  -- 3
    ('Linus',   'Torvalds', 'linus@example.com',    '2025-03-15'),  -- 4
    ('Margaret','Hamilton', 'margaret@example.com', '2025-04-01'),  -- 5
    ('Dennis',  'Ritchie',  'dennis@example.com',   '2025-05-05');  -- 6

-- Book references CategoryId (1=Software, 2=Testing, 3=Process, 4=Languages); no author column.
INSERT INTO dbo.Book (Title, ISBN, PublishedYear, CategoryId, TotalCopies, AvailableCopies, Edition) VALUES
    ('Clean Code',                                     '9780132350885', 2008, 1, 3, 3, 1),  -- 1
    ('Clean Architecture',                             '9780134494166', 2017, 1, 2, 2, 1),  -- 2
    ('Refactoring',                                    '9780201485677', 1999, 1, 2, 1, 2),  -- 3
    ('Patterns of Enterprise Application Architecture','9780321127426', 2002, 1, 1, 1, 1),  -- 4
    ('Test Driven Development',                        '9780321146533', 2002, 2, 2, 2, 1),  -- 5
    ('Extreme Programming Explained',                  '9780321278654', 2004, 3, 1, 0, 2),  -- 6
    ('Design Patterns',                                '9780201633610', 1994, 1, 2, 2, 1),  -- 7
    ('The Pragmatic Programmer',                       '9780201616224', 1999, 1, 4, 3, 1),  -- 8
    ('The Pragmatic Programmer 20th Anniv',            '9780135957059', 2019, 1, 2, 2, 2),  -- 9
    ('Programming Ruby',                               '9780974514055', 2004, 4, 1, 1, 1);  -- 10

-- Authorship via the bridge: the original one-author-per-book links, then real co-authors.
INSERT INTO dbo.BookAuthor (BookId, AuthorId) VALUES
    (1, 1), (2, 1), (3, 2), (4, 2), (5, 3),     -- original single-author links
    (6, 3), (7, 4), (8, 5), (9, 5), (10, 6),
    (7, 3),   -- Design Patterns co-authored (Beck)
    (8, 6);   -- The Pragmatic Programmer co-authored (Thomas)

INSERT INTO dbo.Loan (BookId, MemberId, LoanDate, DueDate, ReturnDate) VALUES
    (3, 1, '2026-06-01', '2026-06-15', NULL),         -- Refactoring, out to Ada
    (6, 2, '2026-05-20', '2026-06-03', NULL),         -- XP Explained, out to Grace (0 available)
    (8, 3, '2026-05-25', '2026-06-08', '2026-06-04'), -- Pragmatic Programmer, returned
    (1, 4, '2026-06-10', '2026-06-24', NULL),         -- Clean Code, out to Linus
    (8, 5, '2026-06-12', '2026-06-26', NULL);         -- Pragmatic Programmer, out to Margaret
GO

-- =====   Intermediate DQL   =====

-- Aggregate functions (operations across rows)
-- COUNT()          > COUNT(somecolumn) != COUNT(*), counting something specific ignores NULL
-- SUM()            > SUM
-- AVG()            > NULL doesnt count
-- MIN(), MAX()     > ye

SELECT 
    COUNT(*) as BookCount, 
    SUM(TotalCopies) AS TotalCopies, 
    AVG(TotalCopies) AS AvgCopies, 
    MIN(PublishedYear) AS Oldest, 
    MAX(PublishedYear) AS Newest 
FROM dbo.Book; 

-- Scalar functions > Transforms a value into a new value
SELECT 
    UPPER(LastName) AS LastName,
    LEN(Email) AS EmailLen,
    CONCAT(FirstName, ' ', LastName) AS FullName,
    DATEDIFF(DAY, JoinedDate, GETDATE()) AS DaysAsMember
FROM dbo.Member;

-- =====   JOINs   =====
-- INNER JOIN
-- LEFT and RIGHT
-- OUTER JOIN (LEFT, RIGHT, FULL)
-- CROSS JOIN


-- Equi-JOIN
SELECT b.Title, c.Name AS Category
FROM dbo.Book AS b
INNER JOIN dbo.Category AS c ON c.CategoryId = b.CategoryId
ORDER BY c.Name, b.Title;

-- Many to Many Join
SELECT b.Title, a.FirstName + ' ' + a.LastName AS Author
FROM dbo.Book AS b
JOIN dbo.BookAuthor ba ON ba.BookId = b.BookId
JOIN dbo.Author a ON a.AuthorId = ba.AuthorId
ORDER BY b.Title, Author;

-- JOINs + GROUP BY + HAVING
SELECT c.Name AS Category, COUNT(*) As Books, SUM(b.TotalCopies) AS Copies
FROM dbo.Book b
JOIN dbo.Category c ON c.CategoryId = b.CategoryId
GROUP BY c.Name HAVING COUNT(*) > 0
ORDER BY Books DESC;

-- LEFT and RIGHT JOINs

-- LEFT JOIN
-- Members and their loans
SELECT m.FirstName, m.LastName, l.LoanId, l.DueDate
FROM dbo.Member AS m
LEFT JOIN dbo.Loan as l ON l.MemberId = m.MemberId
ORDER BY m.LastName;

-- Filtering when null
SELECT m.FirstName, m.LastName
FROM Member m
LEFT JOIN dbo.Loan l ON l.MemberId = m.MemberId
WHERE l.LoanId IS NULL;

-- RIGHT JOIN mirror LEFT
SELECT b.Title, l.LoanId
FROM dbo.Loan l
RIGHT JOIN dbo.Book b ON b.BookId = l.BookId
ORDER BY Title;

-- FULL OUTER vs CROSS 
-- FULL OUTER JOIN > Returns matched rows where they exist as well as unmatched ones
SELECT b.Title, c.Name AS Category
FROM dbo.Book AS b
FULL OUTER JOIN dbo.Category c ON c.CategoryId = b.CategoryId
ORDER BY c.Name;

-- CROSS JOIN
-- Every possible combination of the rows

SELECT a.LastName, c.Name
FROM dbo.Author a
CROSS JOIN dbo.Category c;

-- Subqueries
-- Use a subquerie to filter against a computer value or a set you dont need in the output
-- You can usually do both but tipically the JOIN will be easier to write
SELECT Title, TotalCopies
FROM dbo.Book
WHERE TotalCopies > (SELECT AVG(TotalCopies) FROM dbo.Book);

SELECT FirstName, LastName
FROm dbo.Member
WHERE MemberId IN (
    SELECT MemberId
    FROM dbo.Loan
    WHERE ReturnDate IS NULL
);

-- Correlated subquery: can cause issues
SELECT Title, TotalCopies
FROM dbo.Book b
WHERE TotalCopies > (
    SELECT AVG(TotalCopies)
    FROM dbo.Book b2
    WHERE b.Title = b2.Title
)

SELECT b.Title, (SELECT COUNT(*) FROM dbo.Loan l WHERE l.BookId = b.BookId) AS TimesLoaned
FROM dbo.Book b
ORDER BY TimesLoaned DESC;

-- Dashboard
SELECT m.FirstName + ' ' + m.LastName AS Member, 
    b.Title, 
    c.Name as Category, 
    l.DueDate, 
    DATEDIFF(DAY, l.DueDate, GETDATE()) AS DaysOverdue
FROM dbo.Loan AS l
JOIN dbo.Member m ON m.MemberId = l.MemberId
JOIN dbo.Book b ON b.BookId = l.BookId
JOIN dbo.Category c ON c.CategoryId = b.CategoryId
WHERE l.ReturnDate IS NULL
ORDER BY DaysOverdue;
