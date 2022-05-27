using GradProj.Models.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class HotelRepository : IToureRepository<Hotel>
    {
        private readonly SiteContext db;

        public HotelRepository(SiteContext db)
        {
            this.db = db;
        }
        public void Add(Hotel entity)
        {
            db.Hotels.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var hotel = Find(id);
            db.Hotels.Remove(hotel);
            db.SaveChanges();
        }

        public Hotel Find(Guid id)
        {
            var hotel = db.Hotels.Include(h => h.City).Include(h=>h.Rooms).SingleOrDefault(h => h.Id == id);
            return hotel;
        }

        public List<Hotel> List()
        {
           var hotels= db.Hotels.Include(h => h.City).Include(h => h.Rooms).ToList();
            return hotels;
        }
        public List<Hotel> Search(string query)
        {
            var hotels = db.Hotels.Include(h => h.City).Where(h => h.City.Name.Contains(query)||h.Name.Contains(query)).ToList();
            return hotels;
        }

        public void Update(Guid id, Hotel entity)
        {
            db.Hotels.Update(entity);
            db.SaveChanges();
        }
    }
}
