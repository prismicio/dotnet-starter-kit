using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.FSharp.Core;
using prismic;

namespace prismic.mvc.starter
{
	public static class PrismicLinkResolver
	{
		public static DocumentLinkResolver Get(Api api, string maybeRef, RequestContext requestContext)
		{			
			return prismic.DocumentLinkResolver.For (
				(documentLink) =>
				!documentLink.IsBroken 
				? requestContext.RouteUrlFor("Detail", "Home", new { documentLink.Id, documentLink.Slug })
				: requestContext.RouteUrlFor("BrokenLink", "Home", new {})
			);
		}
	}
}

