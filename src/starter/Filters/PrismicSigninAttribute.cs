using System;
using System.Web;
using System.Web.Mvc;
using prismic;
using System.Web.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace prismic.mvc.starter
{
	public class PrismicSigninAttribute : ActionFilterAttribute, IActionFilter
	{
		void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
		{
			var apiHome = PrismicApiHome.FromConfig ();
			var url = apiHome.Get ()
				.ContinueWith (getApi => {
					if (getApi.IsFaulted) { 
						var innerException = getApi.Exception.Flatten ().InnerExceptions.FirstOrDefault ();
						if (innerException is prismic.Error) {
							var prismicError = (prismic.Error)innerException;
							if (prismicError.Code == prismic.Error.ErrorCode.AUTHORIZATION_NEEDED)
							{
								return prismicError.Message;
							}
							if (prismicError.Code == prismic.Error.ErrorCode.INVALID_TOKEN)
							{
								return prismicError.Message;
							}
						}
						throw getApi.Exception;

					} else {
						return getApi.Result.;
					}
				});

			var clientId = WebConfigurationManager.AppSettings.GetOrThrow ("prismic.clientId");
			var redirectUrl = url.Result.BuildUrlWith(new Dictionary<string, string> { 
				{"client_id", clientId}, 
				{"redirect_uri", GetLoginCallbackUrl(filterContext)}, 
				{"scope", "master+releases"}});

			filterContext.ActionParameters["redirectUrl"] = redirectUrl;

			this.OnActionExecuting(filterContext);
		}

		private string GetLoginCallbackUrl(ActionExecutingContext filterContext)
		{
			return filterContext.RequestContext.AbsoluteRouteUrlFor("LoginCallback", "Home", new { redirect_uri = filterContext.HttpContext.Request.UrlReferrer }); 
		}
	}
}

