using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GradProj.ViewModels
{
    public class RegisterModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(256)]
        public string Email { get; set; }
        [StringLength(256)]
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [MinLength(6,ErrorMessage ="The Length Of The Password Must Be Greater Than 6 Digits!!")]
        public string Password { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        public string ImgUrl { get; set; }
        public IFormFile File { get; set; }
    }
}
