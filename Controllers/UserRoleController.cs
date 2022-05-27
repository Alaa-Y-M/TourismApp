using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRoleController : Controller
    {
        private readonly ApplicationContext db;

        public UserRoleController(ApplicationContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var userRole = db.UserRoles.ToList();
            return View(userRole);
        }
    }
}