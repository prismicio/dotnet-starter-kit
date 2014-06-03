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
			var futureResponse = ctx.Api.Forms ["everything"].Ref (ctx.MaybeRef).PageSize (10).Page (page)
				.SubmitableAsTask ()
				.Submit (); 
				
			ViewBag.Title = "All documents";
			var model = new PrismicResponse(ctx, futureResponse.Result);
			return View (model);
		}

		[PrismicContext("ctx")]
		public ActionResult Detail(PrismicContext ctx, string id, string slug, string refId)
		{
			var futureResponse = ctx.Api.Forms ["everything"]
				.Query(string.Format(@"[[:d = at(document.id, ""{0}"")]]", id))
				.Ref (ctx.MaybeRef)
				.SubmitableAsTask ()
				.Submit (); 

			var document = futureResponse.Result.results.FirstOrDefault ();
			if (document != null && document.slug == slug) {
				ViewBag.Title = "Document detail " + slug;
				return View (new PrismicDocument (ctx, document));
			}
			else if (document != null && document.slugs.Contains(slug)) {
				return RedirectToActionPermanent ("Detail", new { id, document.slug, refId });
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
			var futureResponse = ctx.Api.Forms ["everything"]
				.Query(string.Format(@"[[:d = fulltext(document, ""{0}"")]]", query))
				.Ref (ctx.MaybeRef).PageSize (10).Page (page)
				.SubmitableAsTask ()
				.Submit (); 

			ViewBag.Title = "Search results";
			return View (new PrismicSearchResponse(ctx, futureResponse.Result, q));
		}

		[HttpPost, PrismicSignout]
		public ActionResult Signout ()
		{
			return RedirectToAction ("Index");
		}

		[PrismicSignin]
		public ActionResult Signin (string redirectUrl)
		{
			if (!string.IsNullOrEmpty (redirectUrl)) {
				return Redirect (redirectUrl);
			} else {
				return RedirectToAction ("Index");
			}
		}


		[PrismicSigninCallback]
		public ActionResult LoginCallback(string redirectUrl)
		{
			if (!string.IsNullOrEmpty (redirectUrl)) {
				return Redirect (redirectUrl);
			} else {
				return RedirectToAction ("Index");
			}
		}
	}


}


