using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class RoomRepository: IToureRepository<Room>
    {
            private readonly SiteContext db;

            public RoomRepository(SiteContext db)
            {
                this.db = db;
            }
            public void Add(Room entity)
            {
                db.Rooms.Add(entity);
                db.SaveChanges();
            }
        public List<Room> Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var room = db.Rooms.Where(h => h.Price.ToString().Contains(query) || h.Type.Contains(query)).ToList();
                return room;
            }
            return null;
        }
        public void Delete(Guid id)
            {
                var room = Find(id);
                db.Rooms.Remove(room);
                db.SaveChanges();
            }

            public Room Find(Guid id)
            {
                var room = db.Rooms.AsNoTracking().Include(h => h.Hotel).AsTracking().SingleOrDefault(h => h.Id == id);
                return room;
            }

            public List<Room> List()
            {
                return db.Rooms.AsNoTracking().Include(h => h.Hotel).AsNoTracking().ToList();
            }

            public void Update(Guid id, Room entity)
            {
                db.Rooms.Update(entity);
                db.SaveChanges();
            }
        }
    }
