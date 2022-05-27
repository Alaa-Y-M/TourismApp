using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using GradProj.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FestivalController : Controller
    {
        private readonly IToureRepository<Festival> repo;
        private readonly IToureRepository<City> cityRepo;
        private readonly IWebHostEnvironment hosting;

        public FestivalController(IToureRepository<Festival> repo, IToureRepository<City> cityRepo, IWebHostEnvironment hosting)
        {
            this.repo = repo;
            this.cityRepo = cityRepo;
            this.hosting = hosting;
        }
        // GET: Festival
        public IActionResult Index(int page = 1)
        {
            var query = repo.List().OrderBy(h => h.City.Name);
            var model = PagingList.Create(query, 6, page);
            return View(model);
        }

        // GET: Festival/Details/5
        public ActionResult Details(Guid id)
        {
            var Festival = repo.Find(id);
            return View(Festival);
        }

        // GET: Festival/Create
        public ActionResult Create()
        {
            return View(ViewModel());
        }

        // POST: Festival/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Create(FestivalViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    if (model.CityId == Guid.Empty)
                    {
                        ViewBag.mssg = "Please Select Any City From List";
                        return View(ViewModel());
                    }
                    var Festival = new Festival
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Details = model.Details,
                        City = cityRepo.Find(model.CityId),
                        ImgUrl = await FilePath(model.File)
                    };
                    repo.Add(Festival);
                    return RedirectToAction(nameof(Index));
                }
                return View(ViewModel());
            }
            catch
            {
                return View();
            }
        }

        // GET: Festival/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View(ViewModel(id));
        }

        // POST: Festival/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Edit(FestivalViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid && model != null)
                {
                    if (model.CityId == Guid.Empty)
                    {
                        ViewBag.mssg = "Please Select Any City From List";
                        return View(ViewModel(model.CityId));
                    }
                    var fileName = await FilePath(model.File, model.ImgUrl);
                    var city = cityRepo.Find(model.CityId);
                    var Festival = new Festival
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Details = model.Details,
                        City = city,
                        ImgUrl = fileName
                    };
                    repo.Update(model.Id, Festival);
                    return RedirectToAction(nameof(Index), "Festival");
                }
                return View(ViewModel(model.Id));
            }
            catch
            {
                return View();
            }
        }

        // GET: Festival/Delete/5
        public ActionResult Delete(Guid id)
        {
            var Festival = repo.Find(id);
            return View(Festival);
        }

        // POST: Festival/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Festival Festival)
        {
            try
            {
                // TODO: Add delete logic here
                repo.Delete(Festival.Id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        List<City> FillList()
        {
            var cities = cityRepo.List().ToList();
            cities.Insert(0, new City { Id = Guid.Empty, Name = "--- Please Select Any City ---" });
            return cities;
        }
        FestivalViewModel ViewModel()
        {
            var model = new FestivalViewModel
            {
                Cities = FillList()
            };
            return model;
        }
        FestivalViewModel ViewModel(Guid id)
        {
            var Festival = repo.Find(id);
            var model = new FestivalViewModel
            {
                Id = Festival.Id,
                Name = Festival.Name,
                Details = Festival.Details,
                CityId = Festival.City.Id,
                Cities = FillList(),
                ImgUrl = Festival.ImgUrl
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
                    string FullPath = Path.Combine(hosting.WebRootPath + "/uploads/festival", NewFileName);
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
                    string FullNewPath = Path.Combine(hosting.WebRootPath + "/uploads/festival", NewFileName);
                    string FullOldPath = Path.Combine(hosting.WebRootPath + "/uploads/festival", oldimgurl);
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
        [AllowAnonymous]
        [HttpGet]
        public ActionResult FestivalView(int pageNum = 1)
        {
            var qry = repo.List();
            var model = PagingList.Create(qry, 6, pageNum);
            model.Action = "FestivalView";
            model.PageParameterName = "pageNum";
            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Discover(Guid id)
        {
            var fest = repo.Find(id);
            return View(fest);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Search(string city)
        {
            if (!string.IsNullOrEmpty(city))
            {
                var fest = repo.Search(city);
                var model = PagingList.Create(fest, 6, 1);
                if (model == null)
                {
                    return null;
                }
                return View("FestivalView", model);
            }
            else
                return RedirectToAction("FestivalView");

        }
    }

}