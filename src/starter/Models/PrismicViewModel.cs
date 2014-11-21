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
		readonly prismic.Response response;
		readonly string q;

		public PrismicSearchResponse(PrismicContext ctx, prismic.Response response, string q) : base(ctx)
		{
			this.q = q;
			this.response = response;
		}
		public prismic.Response Response { get { return this.response; } }
		public string Query { get { return this.q; } }
	}

	public class PrismicResponse : PrismicViewModel
	{
		readonly prismic.Response response;
		public PrismicResponse(PrismicContext ctx, prismic.Response response) : base(ctx)
		{
			this.response = response;
		}
		public prismic.Response Response { get { return this.response; } }
	}

	public class PrismicDocument : PrismicViewModel
	{
		readonly prismic.Document document;
		public PrismicDocument(PrismicContext ctx, prismic.Document document) : base(ctx)
		{
			this.document = document;
		}
		public prismic.Document Document { get { return this.document; } }

		public string DocumentAsHtml
		{
			get {
				return this.document.AsHtml (Context.LinkResolver);
			}
		}
	}
}

