using System;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using prismic.mvc.starter;
using System.Threading.Tasks;

namespace prismic.mvc.starter.Controllers
{
	public class HomeController : Controller
	{
		public async Task<ActionResult> Index (int page = 1)
		{
            var ctx = await getContext();
			var response = await ctx.Api.Form("everything").Ref (ctx.MaybeRef).PageSize (10).Page (page).Submit ();
			ViewBag.Title = "All documents";
			var model = new PrismicResponse(ctx, response);
			return View (model);
		}

		public async Task<ActionResult> Detail(string id, string slug)
		{
            var ctx = await getContext();
            var response = await ctx.Api.Form("everything")
				.Query(Predicates.at("document.id", id))
				.Ref (ctx.MaybeRef)
				.Submit (); 

			var document = response.Results.FirstOrDefault ();
			if (document != null && document.Slug == slug) {
				ViewBag.Title = "Document detail " + slug;
				return View (new PrismicDocument (ctx, document));
			}
			else if (document != null && document.Slugs.Contains(slug)) {
				return RedirectToActionPermanent ("Detail", new { id, document.Slug });
			} else {
				ViewBag.Title = "Document not found";
				this.Response.StatusCode = 404;
				return View ("PageNotFound", new PrismicViewModel(ctx));
			}
		}

		public ActionResult BrokenLink()
		{
			return RedirectToAction ("PageNotFound");
		}

		public async Task<ActionResult> PageNotFound()
		{
            var ctx = await getContext();
			ViewBag.Title = "Document not found";
			return View (new PrismicViewModel(ctx));
		}

		public async Task<ActionResult> Search(string q, int page = 1)
		{
            var ctx = await getContext();
			var query = string.IsNullOrWhiteSpace (q) ? string.Empty : q;
			var response = await ctx.Api.Form("everything")
				.Query(Predicates.fulltext("document", query))
				.Ref (ctx.MaybeRef).PageSize (10).Page (page)
				.Submit (); 

			ViewBag.Title = "Search results";
			return View (new PrismicSearchResponse(ctx, response, q));
		}

		public async Task<ActionResult> Preview(string token)
		{
            var ctx = await getContext();
			string url = ctx.Api.PreviewSession (token, ctx.LinkResolver, "/").Result;
			var cookie = new HttpCookie (prismic.Api.PREVIEW_COOKIE, token);
			cookie.Expires = DateTime.Now.AddMinutes (30);
			this.ControllerContext.HttpContext.Response.SetCookie (cookie);
			return Redirect (url);
		}
		
		private async Task<PrismicContext> getContext()
		{
			var endpoint = WebConfigurationManager.AppSettings.Get("prismic.api.url");
			var api = await new PrismicApiHome(endpoint).Get();
			HttpCookie previewCookie = HttpContext.Request.Cookies.Get(prismic.Api.PREVIEW_COOKIE);
			string maybeRef = (previewCookie != null && previewCookie.Value != "")
				? previewCookie.Value
				: api.Master.Reference;
			return new PrismicContext(endpoint, api, maybeRef, PrismicLinkResolver.Get(api, maybeRef, ControllerContext.RequestContext));
		}
	}


}


