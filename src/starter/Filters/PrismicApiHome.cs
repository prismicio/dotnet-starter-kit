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

		public Task<prismic.Api> Get(string accessToken)
		{
			return prismic.Api.Get (this.apiUrl, accessToken, new prismic.DefaultCache(), new PrismicLogger());
		}
		public Task<prismic.Api> Get()
		{
			return this.Get (null);
		}
	}
}

