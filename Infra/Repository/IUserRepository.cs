using DAL.Models;

namespace Infra.Repository;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
}