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
using prismic.extensions;

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

			var accessToken = session.AccessToken.Exists () 
				? session.AccessToken
				: WebConfigurationManager.AppSettings.TryGet ("prismic.token");

			var api = this.apiHome.Get(accessToken)
				.ContinueWith ((Task<prismic.Api.Api> getApi) => {
					if (getApi.IsFaulted) { 
						var innerException = getApi.Exception.Flatten ().InnerExceptions.FirstOrDefault ();
						if (innerException is prismic.Api.AuthorizationNeeded || innerException is prismic.Api.InvalidToken)
						{
							session.Clear();

							var routeDictionary = new RouteValueDictionary {{"action", "Signin"}, {"controller", "Home"}};
							filterContext.Result = new RedirectToRouteResult(routeDictionary);
							return filterContext; // will redirect to signin
						} else {
							throw getApi.Exception;
						}
					} else {
						string refId;
						var maybeRef = 
							filterContext.HttpContext.Request.QueryString.TryGetValue("refId", out refId) 
							? FSharpOption<string>.Some(refId)
							: FSharpOption<string>.Some(getApi.Result.Master.refId);

						filterContext.ActionParameters [contextParameterName] = 
							new PrismicContext (getApi.Result, maybeRef, accessToken, 
								PrismicLinkResolver.Get(getApi.Result, maybeRef, filterContext.RequestContext));
						return filterContext;
					}
				});

			this.OnActionExecuting(api.Result);
		}

	}

}

