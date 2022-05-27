using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using GradProj.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HotelController : Controller
    {
        public static string oldImg { get; set; }
        public static string Success { get; set; }

        private readonly IToureRepository<Hotel> repo;
        private readonly IToureRepository<City> cityRepo;
        private readonly IToureRepository<Room> room;
        private readonly IToureRepository<Customer> crepo;
        private readonly IToureRepository<Booking> brepo;
        private readonly IWebHostEnvironment hosting;
        private readonly UserManager<ApplcationUser> manager;

        public HotelController(IToureRepository<Hotel> repo, IToureRepository<City> cityRepo,
            IToureRepository<Room> room, IToureRepository<Customer> crepo, IToureRepository<Booking> brepo,
            IWebHostEnvironment hosting, UserManager<ApplcationUser> manager)
        {
            this.repo = repo;
            this.cityRepo = cityRepo;
            this.room = room;
            this.crepo = crepo;
            this.brepo = brepo;
            this.hosting = hosting;
            this.manager = manager;
        }
        #region Hotel Codes
        // GET: Hotel
        public IActionResult Index(int page = 1)
        {
            var query = repo.List().OrderBy(h => h.City.Name);
            var model = PagingList.Create(query, 6, page);
            return View(model);
        }

        // GET: Hotel/Details/5
        public ActionResult Details(Guid id)
        {
            var hotel = repo.Find(id);
            return View(hotel);
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
        public async Task<ActionResult> Create(HotelViewModel model)
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
                    var hotel = new Hotel
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Details = model.Details,
                        City = cityRepo.Find(model.CityId),
                        ImgUrl = await FilePath(model.File),
                        Counter = model.Counter
                    };

                    repo.Add(hotel);
                    return RedirectToAction(nameof(Index));
                }
                return View(ViewModel());
            }
            catch
            {
                return View();
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
        public async Task<ActionResult> Edit(HotelViewModel model)
        {
            try
            {
                oldImg = model.ImgUrl;
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
                    var hotel = new Hotel
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Details = model.Details,
                        City = city,
                        ImgUrl = fileName,
                        Counter = model.Counter
                    };
                    repo.Update(model.Id, hotel);
                    return RedirectToAction(nameof(Index), "Hotel");
                }
                return View(ViewModel(model.Id));
            }
            catch
            {
                return View(ViewModel(model.Id));
            }
        }

        // GET: Hotel/Delete/5
        public ActionResult Delete(Guid id)
        {
            var hotel = repo.Find(id);
            return View(hotel);
        }

        // POST: Hotel/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Hotel hotel)
        {
            try
            {
                // TODO: Add delete logic here
                repo.Delete(hotel.Id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult HotelView(int pageNum = 1)
        {
            var qry = repo.List();
            var model = PagingList.Create(qry, 6, pageNum);
            model.Action = "HotelView";
            model.PageParameterName = "pageNum";
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Discover(Guid id)
        {
            RoomNBooing();
            BookingExpiration();
            var hotel = repo.Find(id);
            return View(hotel);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(string city)
        {
            if (!string.IsNullOrEmpty(city))
            {
                var hotel = repo.Search(city);
                var model = PagingList.Create(hotel, 10, 1);
                if (model == null)
                {
                    return null;
                }
                return View("HotelView", model);
            }
            else
                return RedirectToAction("HotelView");
        }
        #endregion
        [AllowAnonymous]
        [HttpGet]
        public ActionResult BookingView()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> BookingView(BookingViewModel model, Guid Id, int pageNum = 1)
        {
            try
            {
                var user = await manager.FindByEmailAsync(model.Email);
                var rom = room.Find(Id);
                if (user == null)
                {
                    ViewBag.message = "Please Login First!!";
                    return View();
                }
                if (rom.IsBusy)
                {
                    ViewBag.mssg = "Please Select Another Because Room Is Busy Now!";
                    return View();
                }
                var Datenow = DateTime.Now;
                if (Datenow > model.CheckOut)
                {
                    ViewBag.mssg = "Please Select A Valid Time!";
                    return View();
                }
                // TODO: Add update logic here
                if (ModelState.IsValid && model != null)
                {
                    rom.IsBusy = true;
                    var cst = crepo.List().Any(c => c.Email == user.Email);
                    var BookingModel = new Booking
                    {
                        CheckIn = model.CheckIn,
                        CheckOut = model.CheckOut,
                        People = model.People,
                        Customer = await IsCustomerExist(cst, model.Email),
                        Room = rom
                    };
                    brepo.Add(BookingModel);
                    Success = "You Booked This Room Successfully & We R Waiting U";
                    var qry = repo.List();
                    var modul = PagingList.Create(qry, 6, pageNum);
                    modul.Action = "HotelView";
                    modul.PageParameterName = "pageNum";
                    return RedirectToAction("HotelView", modul);
                }
                return View();
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
        HotelViewModel ViewModel()
        {
            var model = new HotelViewModel
            {
                Cities = FillList()
            };
            return model;
        }
        HotelViewModel ViewModel(Guid id)
        {
            var hotel = repo.Find(id);
            var model = new HotelViewModel
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Details = hotel.Details,
                CityId = hotel.City.Id,
                Cities = FillList(),
                ImgUrl = hotel.ImgUrl
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
                    string FullPath = Path.Combine(hosting.WebRootPath + "/uploads/hotel", NewFileName);
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
                    string FullNewPath = Path.Combine(hosting.WebRootPath + "/uploads/hotel", NewFileName);
                    string FullOldPath = Path.Combine(hosting.WebRootPath + "/uploads/hotel", oldimgurl);
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
        private async Task<Customer> IsCustomerExist(bool found, string email)
        {
            Customer customer;
            var user = await manager.FindByEmailAsync(email);
            if (!found)
            {
                customer = new Customer
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Phone = user.PhoneNumber
                };
                return customer;
            }
            else
            {
                customer = crepo.List().FirstOrDefault(c => c.Email == email);
                return customer;
            }
        }
        private void BookingExpiration()
        {
            if (brepo.List().Any(b => b.CheckOut < DateTime.Now))
            {
                var expired = brepo.List().Where(b => b.CheckOut < DateTime.Now);
                foreach (var item in expired)
                {
                    var rom = room.List().Where(r => r.Id == item.Room.Id).FirstOrDefault();
                    if (rom != null)
                    {
                        rom.Id = item.Room.Id;
                        rom.IsBusy = false;
                        room.Update(item.Room.Id, rom);
                    }
                }
            }
        }
        private void RoomNBooing()
        {
            //rooms is Busy && Not Found In Booking

            var roomIsBusy = room.List().Where(r => r.IsBusy);

            foreach (var item in roomIsBusy)
            {
                if (!brepo.List().Any(v => v.Room.Id == item.Id))
                {
                    var rom = room.List().Where(r => r.Id == item.Id).FirstOrDefault();
                    if (rom != null)
                    {
                        rom.Id = item.Id;
                        rom.IsBusy = false;
                        room.Update(item.Id, rom);
                    }
                }
            }
        }
    }

}