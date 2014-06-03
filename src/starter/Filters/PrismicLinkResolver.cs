using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.FSharp.Core;
using prismic;
using prismic.extensions;

namespace prismic.mvc.starter
{
	public static class PrismicLinkResolver
	{
		public static Api.DocumentLinkResolver Get(Api.Api api, FSharpOption<string> maybeRef, RequestContext requestContext)
		{			
			return prismic.extensions.DocumentLinkResolver.For (api, 
				(documentLink, maybeBookmarked) =>
				!documentLink.isBroken 
				? requestContext.RouteUrlFor("Detail", "Home", new { documentLink.id, documentLink.slug, refId=maybeRef.GetOrElse(null) })
				: requestContext.RouteUrlFor("BrokenLink", "Home", new { refId=maybeRef.GetOrElse(null) })
			);
		}
	}
}

