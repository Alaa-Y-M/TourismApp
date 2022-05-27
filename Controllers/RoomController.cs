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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
        [Authorize(Roles = "Admin")]
        public class RoomController : Controller
        {
        private readonly IToureRepository<Room> repo;
        private readonly IToureRepository<Hotel> hrepo;
        private readonly IWebHostEnvironment hosting;

            public RoomController(IToureRepository<Room> repo,IToureRepository<Hotel>hrepo, IWebHostEnvironment hosting)
            {
                this.repo = repo;
            this.hrepo = hrepo;
            this.hosting = hosting;
            }
            // GET: Hotel
            public IActionResult Index(int page = 1)
            {
                var query = repo.List();
                var model = PagingList.Create(query, 6, page);
                return View(model);
            }

            // GET: Hotel/Details/5
            public ActionResult Details(Guid id)
            {
                var room = repo.Find(id);
                return View(room);
            }

            // GET: Hotel/Create
            public ActionResult Create()
            {
                return View(ViewModel());
            }

            // POST: Hotel/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            [Obsolete]
            public async Task<ActionResult> Create(RoomViewModel model)
            {
                try
                {
                // TODO: Add insert logic here
                if (model.HotelId == Guid.Empty)
                {
                    ViewBag.mssg = "Please Select Any Hotel From List";
                    return View(ViewModel());
                }
                if (ModelState.IsValid)
                    {
                    var room = new Room
                    {
                        RoomNumber = model.RoomNumber,
                        Price = model.Price,
                        Type = model.Type,
                        NumberOfBeds = model.NumberOfBeds,
                        IsBusy=model.IsBusy,
                        ImgUrl = await FilePath(model.File) ?? string.Empty,
                        Hotel = hrepo.Find(model.HotelId)
                        };
                        repo.Add(room);
                        return RedirectToAction(nameof(Index));
                    }
                    return View(ViewModel());
                }
                catch
                {
                    return View(ViewModel());
                }
            }

            // GET: Hotel/Edit/5
            public ActionResult Edit(Guid id)
            {
            return View(ViewModel(id));
            }
        
        // POST: Hotel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Edit(RoomViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid && model != null)
                {
                    if (model.HotelId == Guid.Empty)
                    {
                        ViewBag.mssg = "Please Select Any Hotel From List";
                        return View(ViewModel(model.Id));
                    }
                    var rom = repo.List().Where(r => r.Id == model.Id).FirstOrDefault();
                    if (rom != null)
                    {
                        var fileName = await FilePath(model.File, model.ImgUrl);
                        rom.Id = model.Id;
                        rom.ImgUrl = fileName;
                        rom.IsBusy = model.IsBusy;
                        rom.NumberOfBeds = model.NumberOfBeds;
                        rom.Type = model.Type;
                        rom.HotelID = model.HotelId;
                        repo.Update(model.Id, rom);
                        return RedirectToAction(nameof(Index), "Room");
                    }
                }
                return View(ViewModel(model.Id));
            }
            catch(Exception ex)
            {
                var c = ex;
                return View();
            }
        }

        // GET: Hotel/Delete/5
        public ActionResult Delete(Guid id)
            {
                var room = repo.Find(id);
                return View(room);
            }

            // POST: Hotel/Delete/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Delete(Room room)
            {
                try
                {
                    // TODO: Add delete logic here
                    repo.Delete(room.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
        List<Hotel> FillList()
        {
            var hotels = hrepo.List().ToList();
            hotels.Insert(0, new Hotel { Id = Guid.Empty, Name = "--- Please Select Ur Hotel ---" });
            return hotels;
        }
        RoomViewModel ViewModel()
        {
            var model = new RoomViewModel
            {
                 Hotels= FillList()
            };
            return model;
        }

        RoomViewModel ViewModel(Guid id)
            {
                var R = repo.Find(id);
                var model = new RoomViewModel
                {
                    Id = R.Id,
                    RoomNumber = R.RoomNumber,
                    NumberOfBeds = R.NumberOfBeds,
                    Price = R.Price,
                    IsBusy=R.IsBusy,
                    Type = R.Type,
                    ImgUrl = R.ImgUrl,
                    HotelId=R.Hotel.Id,
                    Hotels=FillList()
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
                    string FullPath = Path.Combine(hosting.WebRootPath + "/uploads/room", NewFileName);
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
                    string FullNewPath = Path.Combine(hosting.WebRootPath + "/uploads/room", NewFileName);
                    string FullOldPath = Path.Combine(hosting.WebRootPath + "/uploads/room", oldimgurl);
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