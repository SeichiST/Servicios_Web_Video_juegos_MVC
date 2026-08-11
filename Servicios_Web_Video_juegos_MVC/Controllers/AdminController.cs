using Microsoft.AspNetCore.Mvc;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
