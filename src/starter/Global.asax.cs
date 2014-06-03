using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace prismic.mvc.starter
{

	public class MonoRazorViewEngine : RazorViewEngine
	{
		protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
		{
			return base.FileExists(controllerContext, virtualPath.Replace("~", ""));
		}
	}

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes (RouteCollection routes)
		{
			routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");

			routes.MapRoute (
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Home", action = "Index", id = "" }
			);

		}

		public static void RegisterGlobalFilters (GlobalFilterCollection filters)
		{
			filters.Add (new HandleErrorAttribute ());
		}

		protected void Application_Start ()
		{
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new MonoRazorViewEngine());

			AreaRegistration.RegisterAllAreas ();
			RegisterGlobalFilters (GlobalFilters.Filters);
			RegisterRoutes (RouteTable.Routes);
		}
	}
}
