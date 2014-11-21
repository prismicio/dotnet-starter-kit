using System;
using System.Linq;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.FSharp.Core;
using prismic;

namespace prismic.mvc.starter
{
	/// <summary>
	/// This attribute is used to add the prismic context to actions parameters
	/// </summary>
	public class PrismicContextAttribute : ActionFilterAttribute, IActionFilter
	{
		readonly PrismicApiHome apiHome;
		readonly string contextParameterName;
		readonly Func<HttpContextBase, PrismicSession> getSession;

		public PrismicContextAttribute(string contextParameterName)
		{
			Contract.Assert (!string.IsNullOrEmpty (contextParameterName));

			this.contextParameterName = contextParameterName;
			this.apiHome = PrismicApiHome.FromConfig ();
			this.getSession = ctx => new PrismicSession (ctx);
		}

		void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
		{
			var session = getSession (filterContext.HttpContext);
			var accessToken = WebConfigurationManager.AppSettings ["prismic.token"];

			var api = this.apiHome.Get(accessToken)
				.ContinueWith ((Task<prismic.Api> getApi) => {
					if (getApi.IsFaulted) { 
						var innerException = getApi.Exception.Flatten ().InnerExceptions.FirstOrDefault ();
						if (innerException is prismic.Error || ((prismic.Error)innerException).Code == prismic.Error.ErrorCode.INVALID_TOKEN)
						{
							session.Clear();

							var routeDictionary = new RouteValueDictionary {{"action", "Signin"}, {"controller", "Home"}};
							filterContext.Result = new RedirectToRouteResult(routeDictionary);
							return filterContext; // will redirect to signin
						} else {
							throw getApi.Exception;
						}
					} else {
						HttpCookie previewCookie =
							filterContext.HttpContext.Request.Cookies.Get(prismic.Api.PREVIEW_COOKIE);
						string maybeRef = (previewCookie != null && previewCookie.Value != "")
							? previewCookie.Value
							: getApi.Result.Master.Reference;
						Console.WriteLine("Got ref = " + maybeRef);

						filterContext.ActionParameters [contextParameterName] = 
							new PrismicContext (getApi.Result, maybeRef, 
								PrismicLinkResolver.Get(getApi.Result, maybeRef, filterContext.RequestContext));
						return filterContext;
					}
				});

			this.OnActionExecuting(api.Result);
		}

	}

}

