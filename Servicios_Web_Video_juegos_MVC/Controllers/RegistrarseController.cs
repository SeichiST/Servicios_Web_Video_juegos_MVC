using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicios_Web_Video_juegos_MVC.Dto;
using System.Text;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    public class RegistrarseController : Controller
    {
        private readonly string _apiregistrarse =
            "https://localhost:7017/api/RegistrarseAPI";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse (RegistrarseDto dto)
        {
           using (HttpClient cliente = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(dto);
                var contenido = new StringContent(json, Encoding.UTF8, "application/json");

                var respuesta = await cliente.PostAsync(_apiregistrarse, contenido);

                if (!respuesta.IsSuccessStatusCode)
                {
                    string respuestaJson = await respuesta.Content.ReadAsStringAsync();

                    try
                    {
                        var resultado = JsonConvert.DeserializeObject<dynamic>(respuestaJson);
                        ViewBag.Mensaje = resultado?.mensaje ?? "No se pudo realizar el registro";
                    }
                    catch
                    {
                        ViewBag.Mensaje = "No se pudo realizar el registro";
                    }
                    return View("Index", dto);
                }

                ViewBag.Mensaje = "Registro realizado correctamente";
                return RedirectToAction("Index", "Login");
            }

        }
    }
}
