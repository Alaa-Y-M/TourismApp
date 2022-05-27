using GradProj.Models.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class PlaceRepository : IToureRepository<Place>
    {
        private readonly SiteContext db;

        public PlaceRepository(SiteContext db)
        {
            this.db = db;
        }
        public void Add(Place entity)
        {
            db.Places.Add(entity);
            db.SaveChanges();
        }
        public List<Place> Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var places = db.Places.Include(h => h.City).Where(h => h.City.Name.Contains(query) || h.Name.Contains(query)).ToList();
                return places;
            }
            return null;
        }
        public void Delete(Guid id)
        {
            var place = Find(id);
            db.Places.Remove(place);
            db.SaveChanges();
        }

        public Place Find(Guid id)
        {
            var place=db.Places.Include(p=>p.City).SingleOrDefault(p=>p.Id==id);
            return place;
        }

        public List<Place> List()
        {
           return db.Places.Include(b => b.City).ToList();
        }

        public void Update(Guid id, Place place)
        {
            db.Places.Update(place);
            db.SaveChanges();
        }
    }
}
