using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    
    public class PostController : Controller
    {
        public static string emo;
        private readonly IToureRepository<Post> repo;
        private readonly IToureRepository<Customer> crepo;
        private readonly UserManager<ApplcationUser> manager;

        public PostController(IToureRepository<Post> repo, IToureRepository<Customer> crepo,UserManager<ApplcationUser> manager)
        {
            this.repo = repo;
            this.crepo = crepo;
            this.manager = manager;
        }

        // GET: Post/Details/5
        public ActionResult Details(Guid id)
        {
            var Post = repo.Find(id);
            return View(Post);
        }

      
        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post Post,bool returned)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var user = await manager.FindByIdAsync(userId);
                    emo=user.Email;
                    Post.Customer = await IsCustomerExist();
                    repo.Add(Post);
                    if (returned)
                    {
                        return RedirectToAction("RestaurantView","Restaurant");
                    }
                    return RedirectToAction("PlaceView", "Place");
                }
                return RedirectToAction("index","Home");
            }
            catch
            {
                return RedirectToAction("index", "Home");
            }
        }

        // GET: Post/Edit/5
        public ActionResult Edit(Guid id)
        {
            var Post = repo.Find(id);
            return View(Post);
        }

        // POST: Post/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, Post Post)
        {
            if (ModelState.IsValid)
            {
                repo.Update(id, Post);
                return RedirectToAction(nameof(Index),"home");
            }
            return View(Post);
        }

        // GET: Post/Delete/5
        public ActionResult Delete(Guid id)
        {
            var Post = repo.Find(id);
            return View(Post);
        }

        // POST: Post/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, Post Post)
        {
            // TODO: Add delete logic here
            repo.Delete(Post.Id);
            return RedirectToAction(nameof(Index));
        }
        private async Task<Customer> IsCustomerExist()
        {
            Customer customer;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (username != null)
            {
                var exist = crepo.List().Any(c => c.UserName == username);
                if (exist)
                {
                    customer = crepo.List().FirstOrDefault(c => c.UserName == username);
                    return customer;
                }
            }
            var user=await manager.FindByNameAsync(username);
                customer = new Customer
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Phone = user.PhoneNumber,
                    ImgUrl=user.ImgUrl
                };
                return customer;
        }
    }
}
