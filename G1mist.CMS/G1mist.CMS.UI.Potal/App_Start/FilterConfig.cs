using System.Web.Mvc;
using G1mist.CMS.UI.Potal.Filters;

namespace G1mist.CMS.UI.Potal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new G1MistHandleErrorAttribute());
        }
    }
}