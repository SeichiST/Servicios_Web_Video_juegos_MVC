using X.PagedList;
using X.PagedList.Extensions;

namespace Servicios_Web_Video_juegos_MVC.Helpers
{
    public static class PaginacioHelper
    {
        public static IPagedList<T> PaginarLista<T>(IEnumerable<T> lista, int? page, int pageSize = 5)
        {
            int pageNumber = page ?? 1;
            return lista.ToPagedList(pageNumber, pageSize);
        }
    }
}