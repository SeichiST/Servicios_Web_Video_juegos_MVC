using Newtonsoft.Json;
using Servicios_Web_Video_juegos_MVC.Models;

namespace Servicios_Web_Video_juegos_MVC.Helper
{
    public class CarritoHelper
    {
        private const string ClaveSession = "Carrito";

        public static List<CarritoItem> ObtenerCarrito(ISession session)
        {
            var json = session.GetString(ClaveSession);
            if (string.IsNullOrEmpty(json))
                return new List<CarritoItem>();

            return JsonConvert.DeserializeObject<List<CarritoItem>>(json) ?? new List<CarritoItem>();
        }

        public static void GuardarCarrito(ISession session, List<CarritoItem> carrito)
        {
            var json = JsonConvert.SerializeObject(carrito);
            session.SetString(ClaveSession, json);
        }
    }
}
