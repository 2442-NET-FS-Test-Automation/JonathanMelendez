using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Library.Data.Entities;
using Library.Data;

namespace Library.ControllerApi.Services;

public class UserService(LibraryDbContext db, IPasswordHasher<User> hasher) : IUserService
{
    private readonly LibraryDbContext _db = db;
    private readonly IPasswordHasher<User> _hasher = hasher;
    public async Task<string?> RegisterAsync(string username, string password)
    {
        string name = username.Trim();

        if (await _db.Users.AnyAsync(u => u.UserName == name)) return "username is taken";
        User newUser = new() { UserName = name, Role = UserRoles.Consumer };

        newUser.PasswordHash = _hasher.HashPassword(newUser, password);

        _db.Users.Add(newUser);
        return null;
    }
    public async Task<User?> ValidateAsync(string username, string password)
    {
        User? foundUser = await _db.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (foundUser is null) return null;

        var result = _hasher.VerifyHashedPassword(foundUser, foundUser.PasswordHash, password);
        return result == PasswordVerificationResult.Failed ? null : foundUser;
    }
}
