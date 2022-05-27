using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.SiteModels
{
    public class Festival
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
        public string ImgUrl { get; set; }
        [Required]
        public virtual City City { get; set; }
    }
}
