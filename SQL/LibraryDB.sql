USE LibraryDB;
GO

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