using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.SiteModels
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Enter Your Name Please!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Enter Your Email Please!"),DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImgUrl { get; set; }
    }
}
