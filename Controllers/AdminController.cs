using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IToureRepository<Hotel> repo;

        public AdminController(IToureRepository<Hotel> repo)
        {
            this.repo = repo;
        }
        public IActionResult Home()
        {
            var query = repo.List().OrderBy(h => h.City.Name);
            return View(query.Take(6));
        }
    }
}