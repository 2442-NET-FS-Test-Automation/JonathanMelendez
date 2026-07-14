namespace Library.Data.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserRoles Role { get; set; } = UserRoles.Consumer;
}