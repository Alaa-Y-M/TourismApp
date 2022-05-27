using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Concrete
{
    public class CustomerRepository : IToureRepository<Customer>
    {
        private readonly SiteContext db;

        public CustomerRepository(SiteContext db)
        {
            this.db = db;
        }
        public void Add(Customer entity)
        {
            db.Customers.Add(entity);
            db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var Customer = Find(id);
            db.Customers.Remove(Customer);
            db.SaveChanges();
        }

        public Customer Find(Guid id)
        {
            var Customer = db.Customers.SingleOrDefault(c => c.Id == id);
            return Customer;
        }

        public List<Customer> List()
        {
            return db.Customers.ToList();
        }

        public List<Customer> Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var Customer = db.Customers.Where(c => c.UserName.Contains(query)).ToList();
                return Customer;
            }
            return null;
        }

        public void Update(Guid id, Customer Customer)
        {
            db.Customers.Update(Customer);
            db.SaveChanges();
        }
    }
}
