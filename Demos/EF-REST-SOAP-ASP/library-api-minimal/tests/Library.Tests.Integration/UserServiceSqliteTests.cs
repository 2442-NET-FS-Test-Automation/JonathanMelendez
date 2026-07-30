using FluentAssertions;
using Library.ControllerApi.Services;
using Library.Data;
using Library.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Library.Tests.Integration;

public class UserServiceSqliteTests : IDisposable
{
    private class SqliteLibraryDbContext : LibraryDbContext
    {
        public SqliteLibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.Entity<InventoryItem>().Property(i => i.RowVersion)
                .HasDefaultValue(Array.Empty<byte>());
        }
    }

    private readonly SqliteConnection _conn;
    private readonly LibraryDbContext _db;
    private readonly UserService _sut;

    public UserServiceSqliteTests()
    {
        _conn = new SqliteConnection("DataSource=:memory");
        _conn.Open();

        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite(_conn)
            .Options;
        
        _db = new SqliteLibraryDbContext(options);
        _db.Database.EnsureCreated();

        _sut = new UserService(_db, new PasswordHasher<User>());
    }
    public void Dispose()
    {
        _db.Dispose();
        _conn.Dispose();
    }

    [Fact]
    public async Task RegisterAsync_NewUser_PersistsAHasshedPassword()
    {
        // Given
    
        // When
        const string pass = "secure-pass123";
        var error = await _sut.RegisterAsync("grace", pass);

        // Then
        error.Should().BeNull();        

        var newUser = await _db.Users.SingleAsync(u => u.UserName == "grace");

        newUser.Role.Should().Be(UserRoles.Consumer);

        newUser.PasswordHash.Should().NotBeNullOrEmpty();
        newUser.PasswordHash.Should().NotBe(pass);
    }
}