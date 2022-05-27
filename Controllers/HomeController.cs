using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GradProj.Models;
using GradProj.Models.Abstract;
using GradProj.ViewModels;
using GradProj.Models.SiteModels;

namespace GradProj.Controllers
{
    public class HomeController : Controller
    {
        private readonly IToureRepository<Place> repo;
        private readonly IToureRepository<Hotel> hrepo;
        private readonly IToureRepository<Restaurant> rrepo;

        public HomeController(IToureRepository<Place> _repo, IToureRepository<Hotel> Hrepo,IToureRepository<Restaurant> Rrepo)
        {
            repo = _repo;
            hrepo = Hrepo;
            rrepo = Rrepo;
        }

        public IActionResult Index()
        {
            var model = new GeneralViewModel {
                Hotels = hrepo.List().Take(5),
                Places = repo.List().Take(4),
                Restaurants=rrepo.List().Take(3)
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
