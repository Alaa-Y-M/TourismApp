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
    public class PlaceController : Controller
    {
        private readonly IToureRepository<Place> repo;
        private readonly IToureRepository<City> cityRepo;
        private readonly IWebHostEnvironment hosting;
        private readonly IToureRepository<Post> prepo;

        public PlaceController(IToureRepository<Place> repo, IToureRepository<City> cityRepo, IWebHostEnvironment hosting, IToureRepository<Post> prepo)
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
            var place = repo.Find(id);
            return View(place);
        }

        // GET: Hotel/Create
        public ActionResult Create()
        {
            return View(placeView());
        }

        // POST: Hotel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Create(PlaceViewModel model)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    if (model.CityId == Guid.Empty)
                    {
                        ViewBag.mssg = "Please Select Any City From List";
                        return View(placeView());
                    }
                    var place = new Place
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Details = model.Details,
                        City = cityRepo.Find(model.CityId),
                        ImgUrl = await FilePath(model.File),
                        MapUrl = model.MapUrl,
                        Counter = model.Counter
                    };
                    repo.Add(place);
                    return RedirectToAction(nameof(Index));
                }
                return View(placeView());
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
        public async Task<ActionResult> Edit(PlaceViewModel model)
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
                    var place = repo.List().Where(p => p.Id == model.Id).FirstOrDefault();
                    if (place != null)
                    {
                        var fileName = await FilePath(model.File, model.ImgUrl);
                        place.Id = model.Id;
                        place.Name = model.Name;
                        place.Details = model.Details;
                        place.City = cityRepo.Find(model.CityId);
                        place.MapUrl = model.MapUrl;
                        place.ImgUrl = fileName;
                        place.Counter = model.Counter;
                        repo.Update(model.Id, place);
                        return RedirectToAction(nameof(Index));
                    }
                    
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
            var place = repo.Find(id);
            return View(place);
        }

        // POST: Hotel/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Place place)
        {
            try
            {
                // TODO: Add delete logic here
                repo.Delete(place.Id);
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
        PlaceViewModel placeView()
        {
            var model = new PlaceViewModel
            {
                Cities = FillList()
            };
            return model;
        }
        PlaceViewModel EditView(Guid id)
        {
            var place = repo.Find(id);
            var model = new PlaceViewModel
            {
                Id = place.Id,
                Name = place.Name,
                Details = place.Details,
                CityId = place.City.Id,
                Cities = FillList(),
                MapUrl = place.MapUrl,
                ImgUrl = place.ImgUrl
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
                    string FullPath = Path.Combine(hosting.WebRootPath + "/uploads/destination", NewFileName);
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
                    string FullNewPath = Path.Combine(hosting.WebRootPath + "/uploads/destination", NewFileName);
                    string FullOldPath = Path.Combine(hosting.WebRootPath + "/uploads/destination", oldimgurl);
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
        public ActionResult PlaceView(int pageNum = 1)
        {
            var qry = repo.List();
            var model = PagingList.Create(qry, 6, pageNum);
            model.PageParameterName = "pageNum";
            model.Action = "PlaceView";
            var place = new PostPlaceViewModel
            {
                PlacePaging = model,
                Posts = prepo.List()
            };
            return View(place);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Discover(Guid id)
        {
            var place = repo.Find(id);
            return View(place);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Search(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var place = repo.Search(term);
                var model = PagingList.Create(place, 6, 1);
                var mood = new PostPlaceViewModel { PlacePaging = model };
                if (mood == null)
                {
                    return null;
                }
                return View("PlaceView", mood);
            }
            else
                return RedirectToAction("PlaceView");
        }
    }
}