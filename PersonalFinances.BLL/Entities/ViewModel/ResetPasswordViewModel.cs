using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinances.BLL.Entities.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "A Password deve ter pelo menos 6 caracteres.")]
        public string Password { get; set; }
    }

}
