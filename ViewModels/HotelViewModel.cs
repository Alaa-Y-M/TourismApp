using GradProj.Models;
using GradProj.Models.SiteModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.ViewModels
{
    public class HotelViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string ImgUrl { get; set; }
        public IFormFile File { get; set; }
        public Guid CityId { get; set; }
        public int Counter { get; set; }
        public List<City> Cities { get; set; }
    }
}
