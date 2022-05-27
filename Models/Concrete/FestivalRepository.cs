
using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class FestivalRepository : IToureRepository<Festival>
    {
        private readonly SiteContext db;

        public FestivalRepository(SiteContext db)
        {
            this.db = db;
        }
        public void Add(Festival entity)
        {
            db.Festivals.Add(entity);
            db.SaveChanges();
        }
        public List<Festival> Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var fest = db.Festivals.Include(h=>h.City).Where(h => h.City.Name.Contains(query)||h.Name.Contains(query)).ToList();
                return fest;
            }
            return null;
        }

        public void Delete(Guid id)
        {
            var Festival = Find(id);
            db.Festivals.Remove(Festival);
            db.SaveChanges();
        }

        public Festival Find(Guid id)
        {
            var Festival = db.Festivals.Include(h => h.City).SingleOrDefault(h => h.Id == id);
            return Festival;
        }

        public List<Festival> List()
        {
            return db.Festivals.Include(h => h.City).ToList();
        }

        public void Update(Guid id, Festival entity)
        {
            db.Festivals.Update(entity);
            db.SaveChanges();
        }
    }
}
