using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradProj.Models;
using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using GradProj.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BookingController : Controller
    {
        private readonly IToureRepository<Booking> repo;
        private readonly IToureRepository<Room> room;
        private readonly UserManager<ApplcationUser> manager;
        private readonly IToureRepository<Customer> crepo;

        public BookingController(IToureRepository<Booking> repo, IToureRepository<Room> room, UserManager<ApplcationUser> manager,IToureRepository<Customer> crepo)
        {
            this.repo = repo;
            this.room = room;
            this.manager = manager;
            this.crepo = crepo;
        }
        // GET: Booking
        public IActionResult Index(int page = 1)
        {
            RoomNBooing();
            BookingExpiration();
            var query = repo.List();
            var model = PagingList.Create(query, 6, page);
            return View(model);
        }

        // GET: Booking/Details/5
        public ActionResult Details(Guid id)
        {
            var Booking = repo.Find(id);
            return View(Booking);
        }

        // GET: Booking/Create
        public ActionResult Create()
        {
            return View(ViewModel());
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async Task<ActionResult> Create(BookingViewModel model)
        {
            try
            {
                var user = await manager.FindByEmailAsync(model.Email);
                var rom = room.Find(model.RoomId);
                if (model.RoomId == Guid.Empty)
                {
                    ViewBag.mssg = "Please Select Any Room From List";
                    return View(ViewModel());
                }
                if (user == null)
                {
                    ViewBag.message = "Please Login First!!";
                    return View(ViewModel());
                }
                if (EmailExists(model.Email))
                {
                    ViewBag.message = "Email Booked Room later Please Try later!!";
                    return View(ViewModel());
                }
                if (rom.IsBusy)
                {
                    ViewBag.mssg = "Please Select Another Because Room Is Busy Now!";
                    return View(ViewModel());
                }
                // TODO: Add insert logic here
                if (ModelState.IsValid&&model!=null)
                {
                    rom.IsBusy = true;
                    var cst = crepo.List().Any(c => c.Email == user.Email);
                    var BookingModel = new Booking
                    {
                        CheckIn = model.CheckIn,
                        CheckOut = model.CheckOut,
                        People = model.People,
                        Customer =await IsCustomerExist(cst,model.Email),
                        Room = rom
                    };
                    var t = BookingModel.Id;
                    repo.Add(BookingModel);
                    return RedirectToAction(nameof(Index), "Booking");
                }
                return View(ViewModel());
            }
            catch
            {
                return View(ViewModel());
            }
        }

        // GET: Booking/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View(ViewModel(id));
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Obsolete]
        public async  Task<ActionResult> Edit(BookingViewModel model)
        {
            try
            {
                var user = await manager.FindByEmailAsync(model.Email);
                var rom = room.Find(model.RoomId);
                if (model.RoomId == Guid.Empty)
                {
                    ViewBag.mssg = "Please Select Any Room From List";
                    return View(ViewModel(model.Id));
                }
                if (user == null)
                {
                    ViewBag.message = "Please Login First!!";
                    return View(ViewModel(model.Id));
                }
                if (rom.IsBusy)
                {
                    ViewBag.mssg = "Please Select Another Because Room Is Busy Now!";
                    return View(ViewModel(model.Id));
                }
                // TODO: Add update logic here
                if (ModelState.IsValid && model != null)
                {
                    rom.IsBusy = true;
                    var cst = crepo.List().Any(c => c.Email == user.Email);
                    var BookingModel = new Booking
                    {
                        Id=model.Id,
                        CheckIn = model.CheckIn,
                        CheckOut = model.CheckOut,
                        People = model.People,
                        Customer = await IsCustomerExist(cst, model.Email),
                        Room = rom
                    };
                    repo.Update(model.Id, BookingModel);
                    return RedirectToAction(nameof(Index), "Booking");
                }
                return View(ViewModel(model.Id));
            }
            catch
            {
                return View();
            }
        }

        // GET: Booking/Delete/5
        public ActionResult Delete(Guid id)
        {
            var Booking = repo.Find(id);
            return View(Booking);
        }

        // POST: Booking/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Booking Booking)
        {
            try
            {
                // TODO: Add delete logic here
                repo.Delete(Booking.Id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult BookingView(Guid id)
        {
            return View(ViewModel(id));
        }
       
        #region Refliction Methods
        BookingViewModel ViewModel(Guid id)
        {
            var Booking = repo.Find(id);
            var model = new BookingViewModel
            {
                Id = Booking.Id,
                CheckIn = Booking.CheckIn,
                CheckOut = Booking.CheckOut,
                Email = Booking.Customer.Email,
                People = Booking.People,
                Rooms = FillList()
            };
            return model;
        }
        List<Room> FillList()
        {
            var rooms = room.List().ToList();
            rooms.Insert(0, new Room { Id = Guid.Empty, RoomNumber = "--- Choose Your Room ---" });
            return rooms;
        }
        BookingViewModel ViewModel()
        {
            var model = new BookingViewModel
            {
                Rooms = FillList()
            };
            return model;
        }

        private bool EmailExists(string email)
        {
            return repo.List().Any(u => u.Customer.Email == email);
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
                    Phone = user.PhoneNumber,
                    ImgUrl=user.ImgUrl
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
            if (repo.List().Any(b => b.CheckOut < DateTime.Now))
            {
                var expired = repo.List().Where(b => b.CheckOut < DateTime.Now);
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
                if (!repo.List().Any(v => v.Room.Id == item.Id))
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
        #endregion
    }

}