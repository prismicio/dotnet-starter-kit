using System;
using System.Linq;
using System.Collections.Generic;
using prismic;

namespace prismic.mvc.starter
{
	public class PrismicContext
	{
		readonly prismic.Api api;
		readonly string maybeRef;
		readonly prismic.DocumentLinkResolver linkResolver;

		public PrismicContext(){}

		public PrismicContext(prismic.Api api, string maybeRef, prismic.DocumentLinkResolver linkResolver)
		{
			this.api = api;
			this.maybeRef = maybeRef;
			this.linkResolver = linkResolver;
		}
		public prismic.Api Api { get { return this.api; } }
		public string MaybeRef { get { return this.maybeRef; } }

		public prismic.DocumentLinkResolver LinkResolver
		{
			get { return this.linkResolver; }
		}

		public string ResolveLink(prismic.Document document)
		{
			return this.linkResolver.Resolve (document);
		}

	}
}

