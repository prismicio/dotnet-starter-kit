using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.FSharp.Core;
using prismic;
using prismic.extensions;

namespace prismic.mvc.starter
{
	public class PrismicContext
	{
		readonly prismic.Api.Api api;
		readonly FSharpOption<string> maybeRef;
		readonly FSharpOption<string> maybeAccessToken;
		readonly prismic.Api.DocumentLinkResolver linkResolver;

		public PrismicContext(){}

		public PrismicContext(prismic.Api.Api api, FSharpOption<string> maybeRef, FSharpOption<string> maybeAccessToken, prismic.Api.DocumentLinkResolver linkResolver)
		{
			this.api = api;
			this.maybeRef = maybeRef;
			this.maybeAccessToken = maybeAccessToken;
			this.linkResolver = linkResolver;
		}
		public prismic.Api.Api Api { get { return this.api; } }
		public FSharpOption<string> MaybeRef { get { return this.maybeRef; } } 
		public bool HasPrivilegedAccess { get { return maybeAccessToken.Exists (); } }

		public prismic.Api.DocumentLinkResolver LinkResolver
		{
			get { return this.linkResolver; }
		}

		public string ResolveLink(prismic.Api.Document document)
		{
			return this.linkResolver.Apply (document);
		}


		public IEnumerable<prismic.Api.Ref> FutureReleasesRefs
		{
			get {
				Func<FSharpOption<DateTime>, Int64> mapScheduledAt = scheduledAt => scheduledAt.Exists() ? scheduledAt.Value.Ticks : 0;
				return this.api.Refs
					.Select (refKv => refKv.Value)
					.Where (@ref => !@ref.isMasterRef)
					.OrderBy (@ref => mapScheduledAt (@ref.scheduledAt))
					.ToList ();
			}
		}
	}
}

