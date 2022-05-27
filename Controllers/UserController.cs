using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationContext db;
        private readonly IWebHostEnvironment hosting;
        private readonly UserManager<ApplcationUser> manager;

        public UserController(ApplicationContext db,IWebHostEnvironment hosting,UserManager<ApplcationUser> manager)
        {
            this.db = db;
            this.hosting = hosting;
            this.manager = manager;
        }
        public IActionResult Index(int page=1)
        {
            var qry = db.Users.ToList();
            var model = PagingList.Create(qry, 6, page);
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!IsEmailValid(model.Email))
                {
                    ViewBag.emailinValid = "Invalid Email!!!!";
                    return View();
                }
                if (EmailExists(model.Email))
                {
                    ViewBag.emailExist = "Email Exists!!";
                    return View();
                }
                if (UserNameExists(model.UserName))
                {
                    ViewBag.userExists = "User Name Exists!!";
                    return View();
                }
                var user = new ApplcationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed =model.EmailConfirmed,
                    PhoneNumber=model.PhoneNumber,
                    ImgUrl= await FilePath(model.File)
                };
                var result =await manager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("index");
                }
                return View();
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View(EditView(id));
        }
        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var filename = await FilePath(model.File, model.ImgUrl);
                var user = await manager.FindByIdAsync(model.Id);
                user.Id = model.Id;
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.EmailConfirmed = model.EmailConfirmed;
                user.PhoneNumber = model.PhoneNumber;
                user.ImgUrl = filename;
                var pass = new PasswordHasher<ApplcationUser>().HashPassword(user, model.Password);
                user.PasswordHash = pass;
                var result = await manager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("index");
                }
                return View();
            }
            return View();
        }
        private bool IsEmailValid(string email)
        {
            Regex em = new Regex(@"\w+\@\w+\.com|\w+\@\w+\.net");
            return em.IsMatch(email);
        }
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await manager.FindByIdAsync(id);
            var result = await manager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("index");
            }
            return View();
        }
        private bool UserNameExists(string userName)
        {
            return db.Users.Any(u => u.UserName == userName);
        }

        private bool EmailExists(string email)
        {
            return db.Users.Any(u => u.Email == email);
        }
        private UserViewModel EditView(string id)
        {
            var user = db.Users.SingleOrDefault(u => u.Id == id);
            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                ImgUrl=user.ImgUrl
            };
            return model;
        }
        [Obsolete]
        async Task<string> FilePath(IFormFile file)
        {
            string NewFileName = string.Empty;
            if (file != null && file.Length > 0)
            {
                if (isImgValid(file.FileName))
                {
                    string extinsion = Path.GetExtension(file.FileName);
                    NewFileName = Guid.NewGuid().ToString() + extinsion;
                    string FullPath = Path.Combine(hosting.WebRootPath + "/uploads/users", NewFileName);
                    await file.CopyToAsync(new FileStream(FullPath, FileMode.Create));
                }
                return NewFileName;
            }
            else return NewFileName;

        }

        [Obsolete]
        async Task<string> FilePath(IFormFile file, string oldimgurl)
        {
            string NewFileName = string.Empty;
            if (file != null && file.Length > 0)
            {
                if (isImgValid(file.FileName))
                {
                    string extinsion = Path.GetExtension(file.FileName);
                    NewFileName = Guid.NewGuid().ToString() + extinsion;
                    string FullNewPath = Path.Combine(hosting.WebRootPath + "/uploads/users", NewFileName);
                    string FullOldPath = Path.Combine(hosting.WebRootPath + "/uploads/users", oldimgurl);
                    if (FullNewPath != FullOldPath)
                    {

                        System.IO.File.Delete(FullOldPath);
                        await file.CopyToAsync(new FileStream(FullNewPath, FileMode.Create));
                    }
                }
                return NewFileName;
            }
            return oldimgurl;

        }
        bool isImgValid(string filename)
        {
            var extinsion = Path.GetExtension(filename);
            if (extinsion.Contains(".jpg")) return true;
            if (extinsion.Contains(".jpeg")) return true;
            if (extinsion.Contains(".png")) return true;
            if (extinsion.Contains(".gif")) return true;
            if (extinsion.Contains(".bmb")) return true;
            return false;

        }
    }
}