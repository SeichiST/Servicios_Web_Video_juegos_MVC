using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Servicios_Web_Video_juegos_MVC.Helpers;
using Servicios_Web_Video_juegos_MVC.Models;
using Servicios_Web_Video_juegos_MVC.Filters;

namespace Servicios_Web_Video_juegos_MVC.Controllers
{
    [Route("producto")]
    [LoginFilter]
    public class ProductoController : Controller
    {
        private readonly string _apiJuegos = "https://localhost:7017/api/JuegosAPI";
        private readonly string _apiCategorias = "https://localhost:7017/api/CategoriasAPI";

        [HttpGet("inicio")]
        public IActionResult Inicio()
        {
            return View();
        }

        [HttpGet("nosotros")]
        public IActionResult Nosotros()
        {
            return View();
        }

        [HttpGet("contactanos")]
        public IActionResult Contactanos()
        {
            return View();
        }

        [HttpGet("juegos")]
        public async Task<IActionResult> Juegos(string? idCategoria, int? page)
        {
            var listadoJuegos = new List<Juego>();
            var listadoCategorias = new List<Categoria>();

            using (HttpClient cliente = new HttpClient())
            {
                var rptaJuegos = await cliente.GetAsync($"{_apiJuegos}/GetJuegos");
                string contenidoJuegos = await rptaJuegos.Content.ReadAsStringAsync();
                var descompuestoJuegos = JsonConvert.DeserializeObject<List<Juego>>(contenidoJuegos);

                if (descompuestoJuegos != null)
                {
                    listadoJuegos = descompuestoJuegos
                        .Where(j => j.Activo)
                        .ToList();
                }

                var rptaCat = await cliente.GetAsync($"{_apiCategorias}/GetCategorias");
                string contenidoCat = await rptaCat.Content.ReadAsStringAsync();
                var descompuestoCat = JsonConvert.DeserializeObject<List<Categoria>>(contenidoCat);

                if (descompuestoCat != null)
                {
                    listadoCategorias = descompuestoCat;
                }
            }

            //categorias
            if (!string.IsNullOrEmpty(idCategoria))
            {
                listadoJuegos = listadoJuegos.Where(j => j.IdCategoria == idCategoria).ToList();
            }

            ViewBag.LstCategoria = listadoCategorias;
            ViewBag.IdCategoriaSel = idCategoria;

            var pagedList = PaginacioHelper.PaginarLista(listadoJuegos, page, 9);
            return View(pagedList);
        }

        [HttpGet("detalles_juego")]
        public async Task<IActionResult> DetallesJuego(int id)
        {
            var juego = new Juego();

            using (HttpClient cliente = new HttpClient())
            {
                var rpta = await cliente.GetAsync($"{_apiJuegos}/GetJuego/{id}");
                string contenido = await rpta.Content.ReadAsStringAsync();
                var descompuesto = JsonConvert.DeserializeObject<Juego>(contenido);

                if (descompuesto != null)
                {
                    juego = descompuesto;
                }
            }

            return View("detalles_juego",juego);
        }

       
    }
}