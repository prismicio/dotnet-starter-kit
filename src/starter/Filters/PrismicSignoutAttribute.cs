using System;
using System.Web;
using System.Web.Mvc;

namespace prismic.mvc.starter
{
	public class PrismicSignoutAttribute : ActionFilterAttribute, IActionFilter
	{
		readonly Func<HttpContextBase, PrismicSession> getSession;
		public PrismicSignoutAttribute ()
		{
			this.getSession = ctx => new PrismicSession (ctx);
		}

		void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
		{
			var session = getSession (filterContext.HttpContext);
			session.Clear ();

			this.OnActionExecuting(filterContext);
		}
	}
}

