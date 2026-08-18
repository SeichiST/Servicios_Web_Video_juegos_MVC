using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Servicios_Web_Video_juegos_MVC.Filters
{
    public class AdminFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context) {
            var esAdmin = context.HttpContext.Session.GetString("EsAdmin");

            if (esAdmin != "true") {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
        }
    }
}