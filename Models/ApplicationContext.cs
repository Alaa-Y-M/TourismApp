using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace GradProj.Models
{
    public class ApplicationContext:IdentityDbContext<ApplcationUser,ApplicationRole,string>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> option) : base(option)
        {
        }
    }
}
