using Microsoft.AspNetCore.Mvc;

namespace MusikWebApp.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
