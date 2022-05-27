using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class BookingRepository : IToureRepository<Booking>
    {
        private readonly SiteContext db;

        public BookingRepository(SiteContext db)
        {
            this.db = db;
        }
        public void Add(Booking entity)
        {
            db.Bookings.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var Booking = Find(id);
            db.Bookings.Remove(Booking);
            db.SaveChanges();
        }

        public Booking Find(Guid id)
        {
            var Booking = db.Bookings.AsNoTracking().Include(b=>b.Room).ThenInclude(b=>b.Hotel).Include(b=>b.Customer).SingleOrDefault(h => h.Id == id);
            return Booking;
        }

        public List<Booking> List()
        {
            var Bookings = db.Bookings.AsNoTracking().Include(b => b.Room).ThenInclude(b => b.Hotel).Include(b => b.Customer).ToList();
            return Bookings;
        }
        public List<Booking> Search(string query)
        {
            return new List<Booking> ();//Not Used
        }

        public void Update(Guid id, Booking entity)
        {
            db.Bookings.Update(entity);
            db.SaveChanges();
        }
    }
}
