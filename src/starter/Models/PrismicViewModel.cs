using System;
using prismic;
using prismic.extensions;

namespace prismic.mvc.starter
{
	public class PrismicViewModel
	{
		readonly PrismicContext ctx;
		public PrismicViewModel(PrismicContext ctx)
		{
			this.ctx = ctx;
		}
		public PrismicContext Context
		{
			get { return this.ctx; }
		}
	}

	public class PrismicSearchResponse : PrismicViewModel
	{
		readonly prismic.Api.Response response;
		readonly string q;

		public PrismicSearchResponse(PrismicContext ctx, prismic.Api.Response response, string q) : base(ctx)
		{
			this.q = q;
			this.response = response;
		}
		public prismic.Api.Response Response { get { return this.response; } }
		public string Query { get { return this.q; } }
	}

	public class PrismicResponse : PrismicViewModel
	{
		readonly prismic.Api.Response response;
		public PrismicResponse(PrismicContext ctx, prismic.Api.Response response) : base(ctx)
		{
			this.response = response;
		}
		public prismic.Api.Response Response { get { return this.response; } }
	}

	public class PrismicDocument : PrismicViewModel
	{
		readonly prismic.Api.Document document;
		public PrismicDocument(PrismicContext ctx, prismic.Api.Document document) : base(ctx)
		{
			this.document = document;
		}
		public prismic.Api.Document Document { get { return this.document; } }

		public string DocumentAsHtml
		{
			get {
				return this.document.AsHtml (Context.LinkResolver);
			}
		}
	}
}

