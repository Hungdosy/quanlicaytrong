using Microsoft.AspNetCore.Mvc;

namespace HomeGarden_FE.Controllers
{
    public class AppController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Plant(long id) => View();

        public IActionResult Create() => View();

        public IActionResult Edit(long id) => View();

        public IActionResult Areas()
        {
            return View();
        }
    }
}
