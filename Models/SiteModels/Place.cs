using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models
{
    public class Place
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Please Enter The Place Name!!")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
        public string ImgUrl { get; set; }
        public int Counter { get; set; }
        public string MapUrl { get; set; }
        public virtual City City { get; set; }
    }
}
