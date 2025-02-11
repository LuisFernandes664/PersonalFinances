using PersonalFinances.BLL.Entities.Models;
using PersonalFinances.BLL.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.User
{
    public interface IUserService
    {
        Task<UserModel> GetUserByStampEntity(string stampEntity);
        Task<UserModel> UpdateUser(string stampEntity, UpdateUserViewModel model);
        Task ResetPassword(string userIdentifier, string resetType);
        Task RegisterUser(UserModel user);
        Task ConfirmResetPassword(string token, string newPassword);

        Task<UserModel> AuthenticateUser(string email, string password);
        string GenerateJwtToken(UserModel user);


    }
}
