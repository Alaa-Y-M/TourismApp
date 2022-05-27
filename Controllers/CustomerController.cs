using GradProj.Models.Abstract;
using GradProj.Models.SiteModels;
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;

namespace GradProj.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IToureRepository<Customer> repo;

        public CustomerController(IToureRepository<Customer> repo)
        {
            this.repo = repo;
        }
        public IActionResult Index(int page = 1)
        {
            var query = repo.List();
            var model = PagingList.Create(query, 6, page);
            return View(model);
        }
    }
}