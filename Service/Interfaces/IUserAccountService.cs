using Repository.Entities;

namespace Service.Interfaces;

public interface IUserAccountService
{
    public Task<UserAccount> Authenticate(string username, string password);
}