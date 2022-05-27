using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class PostRepository : IToureRepository<Post>
    {
        private readonly SiteContext db;

        public PostRepository(SiteContext db)
        {
            this.db = db;
        }
        public void Add(Post entity)
        {
            db.Posts.Add(entity);
            db.SaveChanges();
        }
        public List<Post> Search(string query)
        {
            return null;
        }
        public void Delete(Guid id)
        {
            var post = Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
        }

        public Post Find(Guid id)
        {
            var post = db.Posts.Include(p => p.Customer).SingleOrDefault(p => p.Id == id);
            return post;
        }

        public List<Post> List()
        {
            return db.Posts.Include(b => b.Customer).ToList();
        }

        public void Update(Guid id, Post post)
        {
            db.Posts.Update(post);
            db.SaveChanges();
        }
    }
}
