using System;
using System.Web.Configuration;
using System.Diagnostics.Contracts;
using Microsoft.FSharp.Core;
using System.Threading.Tasks;
using prismic.extensions;

namespace prismic.mvc.starter
{
	/// <summary>
	/// Prismic API home is the entry point for the prismic.io API 
	/// that sets the caching and logging strategies
	/// </summary>
	public class PrismicApiHome
	{
		readonly PrismicMemoryCache cache = PrismicMemoryCache.Instance;
		readonly Action<string, string> logger = (level, msg) => {
			if (level == "ERROR")
				System.Diagnostics.Trace.TraceError(msg);
			else
				System.Diagnostics.Trace.TraceInformation(msg);
		};

		readonly string apiUrl;

		public PrismicApiHome(string apiUrl)
		{
			Contract.Assert (!string.IsNullOrEmpty (apiUrl));
			this.apiUrl = apiUrl;
		}

		/// <summary>
		/// Returns an entry point for prismic.io API, gets the params froms the config.
		/// </summary>
		/// <returns>The config.</returns>
		public static PrismicApiHome FromConfig()
		{
			return new PrismicApiHome (WebConfigurationManager.AppSettings.GetOrThrow ("prismic.api.url"));
		}

		public Task<prismic.Api.Api> Get(FSharpOption<string> accessToken)
		{
			return prismic.extensions.Api.Get (accessToken, this.apiUrl, cache, this.logger);
		}
		public Task<prismic.Api.Api> Get(string accessToken)
		{
			return prismic.extensions.Api.Get (accessToken, this.apiUrl, cache, this.logger);
		}
		public Task<prismic.Api.Api> Get()
		{
			return prismic.extensions.Api.Get (this.apiUrl, cache, this.logger);
		}
	}
}

