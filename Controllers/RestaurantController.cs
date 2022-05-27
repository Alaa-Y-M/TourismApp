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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RestaurantController : Controller
    {
        private readonly IToureRepository<Restaurant> repo;
        private readonly IToureRepository<City> cityRepo;
        private readonly IWebHostEnvironment hosting;
        private readonly IToureRepository<Post> prepo;

        public RestaurantController(IToureRepository<Restaurant> repo, IToureRepository<City> cityRepo, IWebHostEnvironment hosting, IToureRepository<Post> prepo)
        {
            this.repo = repo;
            this.cityRepo = cityRepo;
            this.hosting = hosting;
            this.prepo = prepo;
        }
        // GET: Hotel
        public ActionResult Index(int page = 1)
        {
            var qry = repo.List();
            var model = PagingList.Create(qry, 6, page);
            return View(model);
        }

        // GET: Hotel/Details/5
        public ActionResult Details(Guid id)
        {
            var restaurant = repo.Find(id);
            return View(restaurant);
        }

        // GET: Hotel/Create
        public ActionResult Create()
        {
            return View(RestView());
        }

        // POST: Hotel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Create(RestaurantViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    if (model.CityId == Guid.Empty)
                    {
                        ViewBag.mssg = "Please Select Any City From List";
                        return View(RestView());
                    }
                    //var rest=Mapper.Mapping.Mapper<RestaurantViewModel, Restaurant>().Map<Restaurant>(model);
                    var restaurant = new Restaurant
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Details = model.Details,
                        City = cityRepo.Find(model.CityId),
                        ImgUrl = await FilePath(model.File)
                    };
                    repo.Add(restaurant);
                    return RedirectToAction(nameof(Index));
                }
                return View(RestView());
            }
            catch
            {
                return View();
            }
        }

        // GET: Hotel/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View(EditView(id));
        }

        // POST: Hotel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Edit(RestaurantViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    if (model.CityId == Guid.Empty)
                    {
                        ViewBag.mssg = "Please Select Any City From List";
                        return View(EditView(model.CityId));
                    }
                    var restaurant = new Restaurant
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Details = model.Details,
                        City = cityRepo.Find(model.CityId),
                        ImgUrl = await FilePath(model.File, model.ImgUrl)
                    };
                    repo.Update(model.Id, restaurant);
                    return RedirectToAction(nameof(Index));
                }
                return View(EditView(model.Id));
            }
            catch
            {
                return View();
            }
        }

        // GET: Hotel/Delete/5
        public ActionResult Delete(Guid id)
        {
            var restaurant = repo.Find(id);
            return View(restaurant);
        }

        // POST: Hotel/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Restaurant restaurant)
        {
            try
            {
                // TODO: Add delete logic here
                repo.Delete(restaurant.Id);
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
        RestaurantViewModel RestView()
        {
            var model = new RestaurantViewModel
            {
                Cities = FillList()
            };
            return model;
        }
        RestaurantViewModel EditView(Guid id)
        {
            var rest = repo.Find(id);
            var model = new RestaurantViewModel
            {
                Id = rest.Id,
                Name = rest.Name,
                Details = rest.Details,
                CityId = rest.City.Id,
                Cities = FillList(),
                ImgUrl = rest.ImgUrl
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
                    string FullPath = Path.Combine(hosting.WebRootPath + "/uploads/restaurant", NewFileName);
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
                    string FullNewPath = Path.Combine(hosting.WebRootPath + "/uploads/restaurant", NewFileName);
                    string FullOldPath = Path.Combine(hosting.WebRootPath + "/uploads/restaurant", oldimgurl);
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
        public ActionResult Discover(Guid id)
        {
            var restaurant = repo.Find(id);
            return View(restaurant);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(string city)
        {
            if (!string.IsNullOrEmpty(city))
            {
                var rest = repo.Search(city);
                var model = PagingList.Create(rest, 6, 1);
                var mood = new PostPlaceViewModel { RestPaging = model };
                if (mood == null)
                {
                    return null;
                }
                return View("RestaurantView", mood);
            }
            else
            {
                return RedirectToAction("RestaurantView");
            }

        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult RestaurantView(int pageNum = 1)
        {
            var qry = repo.List();
            var model = PagingList.Create(qry, 6, pageNum);
            model.Action = "RestaurantView";
            model.PageParameterName = "pageNum";
            var rest = new PostPlaceViewModel
            {
                RestPaging = model,
                Posts = prepo.List()
            };
            return View(rest);
        }
    }
}