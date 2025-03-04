using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.Models
{
    public class NewPasswordModel
    {
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
