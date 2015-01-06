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
	public static class NetHelper
	{
		public static string Post(this string url, IDictionary<string, string> values)
		{	
			var data = new NameValueCollection();
			foreach (var item in values) {
				data.Add (item.Key, item.Value);
			}

			using(var client = new WebClient())
			{
				byte[] responsebytes = client.UploadValues(url, "POST", data);
				return Encoding.UTF8.GetString(responsebytes); // WebException TODO :================
			}
		}


		public static string BuildUrlWith(this string baseUrl, IDictionary<string, string> values)
		{
			var uri = new System.Uri (baseUrl, UriKind.Absolute);
			var httpValueCollection = HttpUtility.ParseQueryString (uri.Query);
			var nameValueCollection = new NameValueCollection ();
			foreach (var item in values) {
				nameValueCollection.Add (item.Key, item.Value); // TODO: encode UTF-8 / use Uri
			}
			httpValueCollection.Add (nameValueCollection);
			var queryString = httpValueCollection.ToString ();
			var builder = new UriBuilder (uri);
			builder.Query = queryString;
			return builder.ToString ();
		}
	}

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
		public static string AbsoluteRouteUrlFor(this RequestContext requestContext, string action, string controller, object routeValues)
		{
			var helper = new UrlHelper(requestContext);
			var scheme = requestContext.HttpContext.Request.Url.Scheme;
			return helper.Action (action, controller, routeValues, scheme);
		}
	}

	public static class NameValueCollectionExt
	{
		public static bool TryGetValue(this NameValueCollection settings, string settingKey, out string value)
		{
			value = settings[settingKey];
			return !string.IsNullOrEmpty (value);
		}
		public static string GetOrThrow(this NameValueCollection settings, string settingKey)
		{
			var setting = settings[settingKey];
			if (!string.IsNullOrEmpty (setting)) 
			{
				return setting;
			}
			throw new KeyNotFoundException ("key is missing : " + settingKey);
		}

	}
}

