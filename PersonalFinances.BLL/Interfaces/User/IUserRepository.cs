using PersonalFinances.BLL.Entities.Models;

namespace PersonalFinances.BLL.Interfaces.User
{
    public interface IUserRepository
    {
        Task<UserModel> GetUserByStampEntity(string stampEntity);
        Task<UserModel> GetUserByEmail(string email);
        Task SaveUser(UserModel user);
        Task UpdateUser(UserModel user);
    }

}
