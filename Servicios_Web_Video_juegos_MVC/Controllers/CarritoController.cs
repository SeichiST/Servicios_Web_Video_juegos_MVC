using Microsoft.AspNetCore.Mvc;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    public class CarritoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
