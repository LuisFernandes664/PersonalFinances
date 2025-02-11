using PersonalFinances.BLL.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.ViewModel
{
    public class UserResponseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public UserResponseViewModel(UserModel user)
        {
            Id = user.StampEntity;
            Name = user.Username;
            Email = user.Email;
            PhoneNumber = user.PhoneNumber;
            CreatedDate = user.CreatedAt;
            UpdatedDate = user.UpdatedAt;
        }

    }

}
