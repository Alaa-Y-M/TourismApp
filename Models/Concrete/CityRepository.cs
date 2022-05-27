using GradProj.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class CityRepository : IToureRepository<City>
    {
        private readonly SiteContext db;

        public CityRepository(SiteContext db)
        {
            this.db = db;
        }
        public void Add(City entity)
        {
            db.Cities.Add(entity);
            db.SaveChanges();
        }
       
        public void Delete(Guid id)
        {
            var city = Find(id);
            db.Cities.Remove(city);
            db.SaveChanges();
        }

        public City Find(Guid id)
        {
            var city = db.Cities.SingleOrDefault(c => c.Id == id);
            return city;
        }

        public List<City> List()
        {
            return db.Cities.ToList();
        }

        public List<City> Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var city = db.Cities.Where(c=>c.Name.Contains(query)).ToList();
                return city;
            }
            return null;
        }

        public void Update(Guid id, City city)
        {
            db.Cities.Update(city);
            db.SaveChanges();
        }
    }
}
