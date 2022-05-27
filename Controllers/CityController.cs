using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.Models.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CityController : Controller
    {
        private readonly IToureRepository<City> repo;

        public CityController(IToureRepository<City> repo)
        {
            this.repo = repo;
        }
        // GET: City
        public ActionResult Index(int page=1)
        {
            var qry = repo.List();
            var model = PagingList.Create(qry, 6, page);
            return View(model);
        }

        // GET: City/Details/5
        public ActionResult Details(Guid id)
        {
            var city = repo.Find(id);
            return View(city);
        }

        // GET: City/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: City/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(City city)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    repo.Add(city);
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: City/Edit/5
        public ActionResult Edit(Guid id)
        {
            var city = repo.Find(id);
            return View(city);
        }

        // POST: City/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id ,City city)
        {
            if (ModelState.IsValid)
            {
                repo.Update(id, city);
                return RedirectToAction(nameof(Index));
            }
            return View(city);
        }

        // GET: City/Delete/5
        public ActionResult Delete(Guid id)
        {
            var city = repo.Find(id);
            return View(city);
        }

        // POST: City/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id,City city)
        {
                // TODO: Add delete logic here
                repo.Delete(city.Id);
                return RedirectToAction(nameof(Index));
        }
    }
}