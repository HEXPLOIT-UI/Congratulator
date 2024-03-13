using Microsoft.AspNetCore.Mvc;

namespace Congratulator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
