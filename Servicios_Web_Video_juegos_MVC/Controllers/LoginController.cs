using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicios_Web_Video_juegos_MVC.Dto;
using System.Text;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _apilogin =
            "https://localhost:7017/api/LoginAPI/Login";
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string password)
        {
            using (HttpClient cliente = new HttpClient())
            {
                var login = new LoginRequestDto { Correo = correo, Password = password };
                string json = JsonConvert.SerializeObject(login);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                var respuesta = await cliente.PostAsync(_apilogin, contenido);

                if (!respuesta.IsSuccessStatusCode)
                {
                    ViewBag.Mensaje = "Correo o contraseña incorrectos";
                    return View("Index");
                }

                string contenidoRespuesta = await respuesta.Content.ReadAsStringAsync();
                var cliente_ = JsonConvert.DeserializeObject<ClienteDto>(contenidoRespuesta);

                HttpContext.Session.SetString("UsuarioLogueado", correo);
                HttpContext.Session.SetInt32("IdCliente", cliente_.IdCliente);

                return RedirectToAction("Inicio", "Producto");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }
    }
}