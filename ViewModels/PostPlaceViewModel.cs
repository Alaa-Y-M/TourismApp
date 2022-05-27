using GradProj.Models;
using GradProj.Models.SiteModels;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.ViewModels
{
    public class PostPlaceViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public PagingList<Place> PlacePaging { get; set; }
        public PagingList<Restaurant> RestPaging { get; set; }
    }
}
