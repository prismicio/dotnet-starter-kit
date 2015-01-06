using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prismic.mvc.starter;
using System.Threading.Tasks;

namespace prismic.mvc.starter.Controllers
{
	public class HomeController : Controller
	{
		public async Task<ActionResult> Index (int page = 1)
		{
            var api = await PrismicApiHome.FromConfig().Get();
            HttpCookie previewCookie = HttpContext.Request.Cookies.Get(prismic.Api.PREVIEW_COOKIE);
            string maybeRef = (previewCookie != null && previewCookie.Value != "")
                ? previewCookie.Value
                : api.Master.Reference;

            var ctx = new PrismicContext(api, maybeRef,
                                PrismicLinkResolver.Get(api, maybeRef, ControllerContext.RequestContext));
			var response = await api.Form("everything").Ref (ctx.MaybeRef).PageSize (10).Page (page)
				.Submit ();
			ViewBag.Title = "All documents";
			var model = new PrismicResponse(ctx, response);
			return View (model);
		}

		[PrismicContext("ctx")]
		public ActionResult Detail(PrismicContext ctx, string id, string slug)
		{
			var futureResponse = ctx.Api.Form("everything")
				.Query(Predicates.at("document.id", id))
				.Ref (ctx.MaybeRef)
				.Submit (); 

			var document = futureResponse.Result.Results.FirstOrDefault ();
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

		[PrismicContext("ctx")]
		public ActionResult PageNotFound(PrismicContext ctx)
		{
			ViewBag.Title = "Document not found";
			return View (new PrismicViewModel(ctx));
		}

		[PrismicContext("ctx")]
		public ActionResult Search(PrismicContext ctx, string q, int page = 1)
		{
			var query = string.IsNullOrWhiteSpace (q) ? string.Empty : q;
			var futureResponse = ctx.Api.Form("everything")
				.Query(Predicates.fulltext("document", query))
				.Ref (ctx.MaybeRef).PageSize (10).Page (page)
				.Submit (); 

			ViewBag.Title = "Search results";
			return View (new PrismicSearchResponse(ctx, futureResponse.Result, q));
		}

		[PrismicContext("ctx")]
		public ActionResult Preview(PrismicContext ctx, string token)
		{
			string url = ctx.Api.PreviewSession (token, ctx.LinkResolver, "/").Result;
			var cookie = new HttpCookie (prismic.Api.PREVIEW_COOKIE, token);
			cookie.Expires = DateTime.Now.AddMinutes (30);
			this.ControllerContext.HttpContext.Response.SetCookie (cookie);
			return Redirect (url);
		}

	}


}


