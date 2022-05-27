using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email Is Required!!")]
        [DataType(DataType.EmailAddress)]
        [StringLength(256)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Is Required!!")]
        [MinLength(6, ErrorMessage = "The Length Of The Password Must Be Greater Than 6 Digits!!")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
