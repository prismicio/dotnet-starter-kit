using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using prismic;
using System.Linq;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

namespace prismic.mvc.starter
{
	public class PrismicSigninCallbackAttribute : ActionFilterAttribute, IActionFilter
	{
		readonly Func<HttpContextBase, PrismicSession> getSession;
		public PrismicSigninCallbackAttribute ()
		{
			this.getSession = ctx => new PrismicSession (ctx);
		}

		void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
		{
			var session = getSession (filterContext.HttpContext);

			var apiHome = PrismicApiHome.FromConfig ();
			var url = apiHome.Get ()
				.ContinueWith (getApi => {
					if (getApi.IsFaulted) { 
						var innerException = getApi.Exception.Flatten ().InnerExceptions.FirstOrDefault ();
						if (innerException is Api.AuthorizationNeeded)
						{
							return ((Api.AuthorizationNeeded)innerException).Data1;
						}
						if (innerException is Api.InvalidToken)
						{
							return ((Api.InvalidToken)innerException).Data1; 
						}
						else throw getApi.Exception;

					} else {
						return getApi.Result.OauthTokenEndpoint;
					}
				});

			string code;
			if (!filterContext.HttpContext.Request.QueryString.TryGetValue ("code", out code)) {
				filterContext.Result = new HttpUnauthorizedResult ("Can't sign you in");
			} else {
				try {
					var accessToken = GetAccessToken (GetLoginCallbackUrl (filterContext), code, url);
					session.Open (accessToken);
					string redirectUrl;
					if (filterContext.HttpContext.Request.QueryString.TryGetValue ("redirect_url", out redirectUrl)) {
						filterContext.ActionParameters ["redirectUrl"] = redirectUrl;
					}
				} catch (Exception ex) {
					filterContext.Result = new HttpUnauthorizedResult ("Can't sign you in");
				}
			}

			this.OnActionExecuting(filterContext);
		}

		private string GetAccessToken(string loginCallbackUrl, string code, Task<string> url)
		{
			var response = url.Result.Post(new Dictionary<string, string> { 
				{"grant_type", "grant_type"}, 
				{"code", code}, 
				{"redirect_uri", loginCallbackUrl},
				{"client_id", WebConfigurationManager.AppSettings.GetOrThrow ("prismic.clientId")},
				{"client_secret", WebConfigurationManager.AppSettings.GetOrThrow ("prismic.clientSecret")}
			});

			var accessToken = new JavaScriptSerializer().Deserialize<IDictionary<string, string>>(response)["access_token"];
			return accessToken;
		}

		private string GetLoginCallbackUrl(ActionExecutingContext filterContext)
		{
			return filterContext.RequestContext.AbsoluteRouteUrlFor("LoginCallback", "Home", new { redirect_uri = filterContext.HttpContext.Request.UrlReferrer }); 
		}
	}



}

