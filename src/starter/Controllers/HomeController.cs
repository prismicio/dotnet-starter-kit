using System.Linq;
using System.Web.Mvc;
using prismic.extensions;
using prismic.mvc.starter;

namespace prismic.mvc.starter.Controllers
{
	public class HomeController : Controller
	{
		[PrismicContext("ctx")]
		public ActionResult Index (PrismicContext ctx, int page = 1)
		{
			System.Diagnostics.Debug.WriteLine("yo, index");

			var futureResponse = ctx.Api.Form("everything").Ref (ctx.MaybeRef).PageSize (10).Page (page)
				.Submit (); 
				
			ViewBag.Title = "All documents";
			var model = new PrismicResponse(ctx, futureResponse.Result);
			return View (model);
		}

		[PrismicContext("ctx")]
		public ActionResult Detail(PrismicContext ctx, string id, string slug, string refId)
		{
			var futureResponse = ctx.Api.Form("everything")
				.Query(string.Format(@"[[:d = at(document.id, ""{0}"")]]", id))
				.Ref (ctx.MaybeRef)
				.Submit (); 

			var document = futureResponse.Result.Results.FirstOrDefault ();
			if (document != null && document.Slugs[0] == slug) {
				ViewBag.Title = "Document detail " + slug;
				return View (new PrismicDocument (ctx, document));
			}
			else if (document != null && document.Slugs.Contains(slug)) {
				string s = document.Slugs[0];
				return RedirectToActionPermanent ("Detail", new { id, s, refId });
			} else {
				ViewBag.Title = "Document not found";
				this.Response.StatusCode = 404;
				return View ("PageNotFound", new PrismicViewModel(ctx));
			}
		}

		public ActionResult BrokenLink(string refId)
		{
			return RedirectToAction ("PageNotFound", new { refId });
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
				.Query(string.Format(@"[[:d = fulltext(document, ""{0}"")]]", query))
				.Ref (ctx.MaybeRef).PageSize (10).Page (page)
				.Submit (); 

			ViewBag.Title = "Search results";
			return View (new PrismicSearchResponse(ctx, futureResponse.Result, q));
		}

	}


}


