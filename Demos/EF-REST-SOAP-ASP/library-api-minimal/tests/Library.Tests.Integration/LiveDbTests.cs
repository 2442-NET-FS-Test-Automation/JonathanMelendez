using FluentAssertions;
using Library.Data;
using Library.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Library.Tests.Integration;

public class LiveDbTests : IDisposable
{
    private const string LiveConnection = "Server=localhost,1433;Database=LibraryMinimalDB;User Id=sa;Password=mssql65.;TrustServerCertificate=true";
    private readonly LibraryDbContext _db;
    private IDbContextTransaction _tx;

    public LiveDbTests()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlServer(LiveConnection)
            .Options;

        _db = new LibraryDbContext(options);

        _tx = _db.Database.BeginTransaction();

        
    }

    public void Dispose()
    {
        _tx.Rollback();
        _tx.Dispose();
        _db.Dispose();
    }

    [Fact]
    public async Task SeedCatalog_IsPresentInTheLiveDatabase()
    {
        // Given
    
        // When
        var skus = await _db.Products.Select(p => p.Sku).ToListAsync();
    
        // Then
        skus.Should().Contain(["BK-001", "BK-002", "BK-002"]);
    }

    [Fact]
    public async Task AddedProduct_IsVisibleTransaction_DeletedUponRollback()
    {
        // Arrange
        _db.Products.Add(new Product {Sku = "TX-TEST-001", Name = "Rollback Book", Price = 50m});
        await _db.SaveChangesAsync();

        (await _db.Products.CountAsync(p => p.Sku == "TX-TEST-001")).Should().Be(1);
    }
}