using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly ApplicationContext db;

        public RoleController(ApplicationContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View(db.Roles.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (RoleNameExists(model.RoleName))
                {
                    ViewBag.roleExists = "Role Name Exists!!";
                    return View();
                }
                var role = new ApplicationRole
                {
                   Name=model.RoleName
                };
                db.Roles.Add(role);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View(EditView(id));
        }
        [HttpPost]
        public IActionResult Edit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole
                {
                    Id = model.Id,
                    Name=model.RoleName
                };
                db.Roles.Update(role);
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return View();
        }
       
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var role = db.Roles.SingleOrDefault(u => u.Id == id);
            db.Roles.Remove(role);
            db.SaveChanges();
            return RedirectToAction("index");
        }
        private bool RoleNameExists(string role)
        {
            return db.Roles.Any(u => u.Name == role);
        }

       
        private RoleViewModel EditView(string id)
        {
            var role = db.Roles.SingleOrDefault(u => u.Id == id);
            var model = new RoleViewModel
            {
                Id = role.Id,
                RoleName=role.Name
            };
            return model;
        }
    }
}