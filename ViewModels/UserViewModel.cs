using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.ViewModels
{
    public class UserViewModel
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email{ get; set; }
        [Required]
        public string Password{ get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber{ get; set; }
        public IFormFile File{ get; set; }
        public string ImgUrl{ get; set; }
    }
}
