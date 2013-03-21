using System.Web;
using System.Web.Mvc;

namespace DolarBlue.TileUpdate.Web
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}