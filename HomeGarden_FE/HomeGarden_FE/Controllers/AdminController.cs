using Microsoft.AspNetCore.Mvc;

namespace HomeGarden_FE.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();

        }
    }
}
