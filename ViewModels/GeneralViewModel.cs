using GradProj.Models;
using GradProj.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.ViewModels
{
    public class GeneralViewModel
    {
        public IEnumerable<Place> Places { get; set; }
        public IEnumerable<Hotel> Hotels { get; set; }
        public IEnumerable<Restaurant> Restaurants { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public Post Post { get; set; }
    }
}
