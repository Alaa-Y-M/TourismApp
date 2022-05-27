using GradProj.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models
{
    public class City
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Please Enter City Name!")]
        public string Name { get; set; }
        public virtual IEnumerable<Hotel> Hotels { get; set; }
        public virtual IEnumerable<Place> Places { get; set; }
        public virtual IEnumerable<Festival> Festivals { get; set; }
        public virtual IEnumerable<Restaurant> Restaurants { get; set; }
    }
}
