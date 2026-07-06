-- Parking Lot*******
-- *                *
-- *                *
--- *****************



-- Comment can be done single line with --
-- Comment can be done multi line with /* */

/*
DQL - Data Query Language
Keywords:

SELECT - retrieve data, select the columns from the resulting set
FROM - the table(s) to retrieve data from
WHERE - a conditional filter of the data
GROUP BY - group the data based on one or more columns
HAVING - a conditional filter of the grouped data
ORDER BY - sort the data
*/


-- BASIC CHALLENGES
-- List all customers (full name, customer id, and country) who are not in the USA
SELECT 
    FirstName + ' ' + LastName AS FullName,
    CustomerId,
    Country
FROM dbo.Customer
WHERE Country != 'USA';

-- List all customers from Brazil
SELECT *
FROM dbo.Customer
WHERE Country = 'Brazil';

-- List all sales agents
SELECT * 
FROM dbo.Employee
WHERE Title = 'Sales Support Agent';

-- SELECT * FROM employee WHERE title LIKE '%Agent%;
SELECT * 
FROM dbo.Employee
WHERE Title LIKE '%Agent%';

-- Retrieve a list of all countries in billing addresses on invoices
SELECT BillingCountry
FROM dbo.Invoice;

-- Retrieve how many invoices there were in 2023, and what was the sales total for that year?
SELECT COUNT(*) AS InvoiceCount, SUM(Total) AS SalesTotal
FROM dbo.Invoice
WHERE Year(InvoiceDate) = 2023;

-- (challenge: find the invoice count sales total for every year using one query)
SELECT YEAR(InvoiceDate) AS Year, COUNT(*) AS InvoiceCount, SUM(Total) AS SalesTotal
FROM dbo.Invoice
GROUP BY YEAR(InvoiceDate);

-- how many line items were there for invoice #37
SELECT COUNT(Quantity) AS TotalItems
FROM dbo.InvoiceLine
WHERE InvoiceId = 37;

-- how many invoices per country? BillingCountry  # of invoices
SELECT BillingCountry, COUNT(*) AS TotalInvoices
FROM dbo.Invoice
GROUP BY BillingCountry;

-- Retrieve the total sales per country, ordered by the highest total sales first.
SELECT BillingCountry, SUM(Total) AS TotalSales
FROM dbo.Invoice
GROUP BY BillingCountry
ORDER BY TotalSales DESC;

-- JOINS CHALLENGES
-- (inner keyword is optional for inner join)
-- Every Album by Artist
SELECT art.Name AS ArtistName, alb.Title AS AlbumTitle
FROM dbo.Artist art
JOIN dbo.Album alb ON alb.ArtistId = art.ArtistId;

-- All songs of the rock genre
SELECT t.Name AS RockSong
FROM dbo.Track t
JOIN dbo.Genre g ON t.GenreId = g.GenreId
WHERE g.Name = 'Rock';

-- Show all invoices of customers from brazil (mailing address not billing)
SELECT * FROM dbo.Invoice i
JOIN dbo.Customer c ON i.CustomerId = c.CustomerId
WHERE c.Country = 'Brazil';

-- Show all invoices together with the name of the sales agent for each one
SELECT 
    i.InvoiceId, i.InvoiceDate, 
    c.FirstName + ' ' + c.LastName AS CustomerName, 
    BillingCity, Total,
    e.FirstName + ' ' + e.LastName AS EmployeeName FROM dbo.Invoice i
JOIN dbo.Customer c ON i.CustomerId = c.CustomerId
JOIN dbo.Employee e ON c.SupportRepId = e.EmployeeId;

-- Which sales agent made the most sales in 2023?
SELECT TOP 1 e.FirstName + ' ' + e.LastName AS EmployeeName, SUM(i.Total) AS TotalSales
FROM dbo.Invoice i
JOIN dbo.Customer c ON i.CustomerId = c.CustomerId
JOIN dbo.Employee e ON c.SupportRepId = e.EmployeeId
WHERE Year(i.InvoiceDate) = 2023
GROUP BY e.FirstName + ' ' + e.LastName
ORDER BY TotalSales DESC;

-- How many customers are assigned to each sales agent?
SELECT e.FirstName + ' ' + e.LastName AS EmployeeName, COUNT(*) AS TotalClients
FROM dbo.Employee e
JOIN dbo.Customer c ON e.EmployeeId = c.SupportRepId
GROUP BY e.FirstName + ' ' + e.LastName;

-- Which track was purchased the most in 2021?
SELECT TOP 1 t.Name AS TrackName, SUM(il.Quantity) AS TimesPurchased
FROM dbo.Track t
JOIN dbo.InvoiceLine il ON t.TrackId = il.TrackId
Join dbo.Invoice i ON il.InvoiceId = i.InvoiceId
WHERE YEAR(i.InvoiceDate) = 2021
GROUP BY t.Name
ORDER BY TimesPurchased DESC;

-- Show the top three best selling artists.
SELECT TOP 3 ar.Name AS ArtistName, SUM(i.Quantity) AS TotalSales
FROM dbo.Artist ar 
JOIN dbo.Album al ON ar.ArtistId = al.ArtistId
JOIN dbo.Track t ON al.AlbumId = t.AlbumId
JOIN dbo.InvoiceLine i ON t.TrackId = i.TrackId
GROUP BY ar.Name
ORDER BY TotalSales DESC;

-- Which customers have the same initials as at least one other customer?
SELECT DISTINCT c1.CustomerId, c1.FirstName, c1.LastName, c2.CustomerId, c2.FirstName, c2.LastName
FROM dbo.Customer c1
JOIN dbo.Customer c2 
    ON LEFT(c1.FirstName, 1) = LEFT(c2.FirstName, 1) 
    AND LEFT(c1.LastName, 1) = LEFT(c2.LastName, 1)
    AND c1.CustomerId != c2.CustomerId;


-- Which countries have the most invoices?
SELECT TOP 10 BillingCountry, COUNT(*) AS TotalInvoices
FROM dbo.Invoice
GROUP BY BillingCountry
ORDER BY TotalInvoices DESC;

-- Which city has the customer with the highest sales total?
SELECT TOP 1 c.City, c.FirstName + ' ' + c.LastName AS CustomerName, SUM(i.Total) AS TotalSales
FROM dbo.Customer c
JOIN dbo.Invoice i ON c.CustomerId = i.CustomerId
GROUP BY c.FirstName + ' ' + c.LastName, c.City
ORDER BY TotalSales DESC;


-- Who is the highest spending customer?
SELECT TOP 1 c.FirstName + ' ' + c.LastName AS CustomerName, SUM(i.Total) AS TotalSales
FROM dbo.Customer c
JOIN dbo.Invoice i ON c.CustomerId = i.CustomerId
GROUP BY c.FirstName + ' ' + c.LastName
ORDER BY TotalSales DESC;

-- Return the email and full name of of all customers who listen to Rock.
SELECT DISTINCT c.FirstName + ' ' + c.LastName AS CustomerName, c.Email
FROM dbo.Customer c
JOIN dbo.Invoice i ON c.CustomerId = i.CustomerId
JOIN dbo.InvoiceLine il ON i.InvoiceId = il.InvoiceId
JOIN dbo.Track t ON il.TrackId = t.TrackId
JOIN dbo.Genre g ON t.GenreId = g.GenreId
WHERE g.Name = 'Rock';

-- Which artist has written the most Rock songs?
SELECT TOP 1 ar.Name AS ArtistName, COUNT(*) AS TotalRockTracks
FROM dbo.Artist ar
JOIN dbo.Album al ON ar.ArtistId = al.ArtistId
JOIN dbo.Track t ON al.AlbumId = t.AlbumId
JOIN dbo.Genre g ON t.GenreId = g.GenreId
WHERE g.Name = 'Rock'
GROUP BY ar.Name
ORDER BY TotalRockTracks DESC;

-- Which artist has generated the most revenue?
SELECT TOP 1 ar.Name, SUM(il.Quantity * il.UnitPrice) AS TotalRevenue
FROM dbo.Artist ar
JOIN dbo.Album al ON ar.ArtistId = al.ArtistId
JOIN dbo.Track t ON al.AlbumId = t.AlbumId
JOIN dbo.InvoiceLine il ON t.TrackId = il.TrackId
JOIN dbo.Invoice i ON il.InvoiceId = i.InvoiceId
GROUP BY ar.Name
ORDER BY TotalRevenue DESC;


-- ADVANCED CHALLENGES
-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- 1. which artists did not make any albums at all?
SELECT Name AS ArtistName FROM dbo.Artist WHERE ArtistId NOT IN (SELECT ArtistId FROM dbo.Album);

-- 2. which artists did not record any tracks of the Latin genre?
SELECT Name AS ArtistName FROM dbo.Artist WHERE ArtistId NOT IN (
    SELECT ar.ArtistId FROM dbo.Artist ar 
    JOIN dbo.Album al ON ar.ArtistId = al.ArtistId
    JOIN dbo.Track t ON al.AlbumId = t.AlbumId
    JOIN dbo.Genre g ON t.GenreId = g.GenreId
    WHERE g.Name = 'Latin'
);

-- 3. which video track has the longest length? (use media type table)
SELECT TOP 1 * FROM dbo.Track WHERE MediaTypeId IN 
    (SELECT MediaTypeId FROM dbo.MediaType WHERE Name LIKE '%video%')
ORDER BY Milliseconds DESC;


-- 4. boss employee (the one who reports to nobody)
SELECT FirstName + ' ' + LastName AS Boss FROM dbo.Employee WHERE ReportsTo IS NULL;


-- 5. how many audio tracks were bought by German customers, and what was
--    the total price paid for them?
SELECT SUM(il.Quantity) AS TotalTracks, SUM(il.UnitPrice * il.Quantity) AS TotalPrice -- was using i.Total but thats wrong
FROM dbo.InvoiceLine il
JOIN dbo.Invoice i ON il.InvoiceId = i.InvoiceId
JOIN dbo.Customer c ON i.CustomerId = c.CustomerId
JOIN dbo.Track t ON il.TrackId = t.TrackId
JOIN dbo.MediaType m ON t.MediaTypeId = m.MediaTypeId
WHERE c.Country = 'Germany' AND m.Name LIKE '%audio%';


-- 6. list the names and countries of the customers supported by an employee
--    who was hired younger than 35.
SELECT FirstName + ' ' + LastName AS CustomerName, Country FROM dbo.Customer WHERE SupportRepId IN
    (SELECT EmployeeId FROM dbo.Employee WHERE DATEDIFF(YEAR, BirthDate, HireDate) < 35);


-- DML exercises

-- 1. insert two new records into the employee table.
INSERT INTO dbo.Employee (LastName, FirstName, Title, ReportsTo, BirthDate, HireDate, Address, City, State, Country, PostalCode, Phone, Fax, Email)
VALUES 
('Melendez', 'Jonathan', 'IT Staff', 6, '2003-09-11', '2026-06-26', 'Mi casa', 'Guadalajara', 'Jalisco', 'Mexico', 'yea lol', '+52 3318930929', '+1 (403) 456-8485', 'superjon007@gmail.com'),
('Martinez', 'Takis', 'IT Staff', 6, '2003-10-23', '2026-06-26', 'Su casa', 'Guadalajara', 'Jalisco', 'Mexico', 'yea lol', '+52 5677889090', '+1 (403) 456-8485', 'takis@hotmail.com');


-- 2. insert two new records into the tracks table.
INSERT INTO dbo.Artist(Name) VALUES ('Porter Robinson'); -- Got ID 276
INSERT INTO dbo.Album(Title, ArtistId) VALUES ('SMILE!', 276); -- Got ID 348

INSERT INTO dbo.Track (Name, AlbumId, MediaTypeId, GenreId, Composer, Milliseconds, Bytes, UnitPrice)
VALUES
('Perfect Pinterest Garden', 348, 5, 9, 'Porter Robinson', 154000, 3192832, 1.19),
('Cheerleader', 348, 5, 9, 'Porter Robinson', 253000, 5248000, 1.19);
-- Pop is ID 9 - AAC Audio is ID 5
 

-- 3. update customer Aaron Mitchell's name to Robert Walter
UPDATE dbo.Customer 
SET FirstName = 'Robert', LastName = 'Walter'
WHERE FirstName = 'Aaron' AND LastName = 'Mitchell';
-- CustomerID = 32
SELECT * FROM dbo.Customer WHERE CustomerId = 32;


-- 4. delete one of the employees you inserted.
DELETE FROM dbo.Employee WHERE Email = 'takis@hotmail.com';


-- 5. delete customer Robert Walter.
DELETE FROM dbo.InvoiceLine WHERE InvoiceId IN (
    SELECT InvoiceId FROM dbo.Invoice WHERE CustomerId =(
        SELECT CustomerId FROM dbo.Customer WHERE FirstName = 'Robert' AND LastName = 'Walter'
    )
) -- Removed 38 InvoiceLines
DELETE FROM dbo.Invoice WHERE CustomerId = (
    SELECT CustomerId FROM dbo.Customer WHERE FirstName = 'Robert' AND LastName = 'Walter'
); -- Removed 7 Invoices
DELETE FROM dbo.Customer WHERE FirstName = 'Robert' AND LastName = 'Walter';
-- Succeded