using PersonalFinances.BLL.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Interfaces.Repository
{
    public interface IPasswordResetRepository
    {
        Task<PasswordResetRequestModel> GetByToken(string token);
        Task SaveAsync(PasswordResetRequestModel resetRequest);
        Task UpdateAsync(PasswordResetRequestModel request);
        Task DeleteAsync(string userID);
    }

}
