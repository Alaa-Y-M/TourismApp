using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string ID { get; set; }
        public string Token { get; set; }
        [Required(ErrorMessage ="Password Is Required!"),DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage ="Two Passwords Are Not Equal !!")]
        public string ConfirmPassword { get; set; }
    }
}
