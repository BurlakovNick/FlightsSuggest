using Microsoft.AspNetCore.Mvc;

namespace FlightsSuggest.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
