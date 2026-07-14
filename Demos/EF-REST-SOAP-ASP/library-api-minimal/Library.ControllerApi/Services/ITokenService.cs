using Library.Data.Entities;

namespace Library.ControllerApi.Services;

public interface ITokenService
{
    string Issue(string user, UserRoles role);
}