using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using prismic;

namespace prismic.mvc.starter
{

	public static class RequestContextExt
	{
		private static RouteValueDictionary Extend(this RouteValueDictionary dest, IEnumerable<KeyValuePair<string, object>> src)
		{
			src.ToList().ForEach(x => { dest[x.Key] = x.Value; });
			return dest;
		}

		public static string RouteUrlFor(this RequestContext requestContext, string action, string controller, object routeValues)
		{
			var values = 
				new RouteValueDictionary {{"action", action}, {"controller", controller}}
				.Extend (new RouteValueDictionary(routeValues));

			var helper = new UrlHelper(requestContext);
			return helper.RouteUrl (values);
		}

	}

}

