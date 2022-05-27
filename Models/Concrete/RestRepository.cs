using GradProj.Models.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class RestRepository : IToureRepository<Restaurant>
    {
        private readonly SiteContext db;

        public RestRepository(SiteContext db)
        {
            this.db = db;
        }
        public void Add(Restaurant entity)
        {
            db.Restaurants.Add(entity);
            db.SaveChanges();
        }
        public List<Restaurant> Search(string query)
        {
            var rest = db.Restaurants.Include(r => r.City).Where(h => h.City.Name.Contains(query) || h.Name.Contains(query)).ToList();
            return rest;
        }
        public void Delete(Guid id)
        {
            var rest = Find(id);
            db.Restaurants.Remove(rest);
            db.SaveChanges();
        }

        public Restaurant Find(Guid id)
        {
            var rest = db.Restaurants.Include(p => p.City).SingleOrDefault(p => p.Id == id);
            return rest;
        }

        public List<Restaurant> List()
        {
            return db.Restaurants.Include(b => b.City).ToList();
        }

        public void Update(Guid id, Restaurant rest)
        {
            db.Restaurants.Update(rest);
            db.SaveChanges();
        }
    }
}
