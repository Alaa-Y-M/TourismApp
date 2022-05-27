using Microsoft.EntityFrameworkCore;
using GradProj.Models.SiteModels;
using GradProj.ViewModels;

namespace GradProj.Models
{
    public class SiteContext:DbContext
    {
        public SiteContext(DbContextOptions<SiteContext> options):base(options)
        {
        }
        public DbSet<City> Cities { set; get; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Festival> Festivals { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}
