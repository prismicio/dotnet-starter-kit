using System;
using System.Web.Configuration;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

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
			return new PrismicApiHome (WebConfigurationManager.AppSettings.Get ("prismic.api.url"));
		}

		public async Task<prismic.Api> Get(string accessToken)
		{
			return await prismic.Api.Get (this.apiUrl, accessToken, new prismic.DefaultCache(), new PrismicLogger());
		}
		public async Task<prismic.Api> Get()
		{
            var accessToken = WebConfigurationManager.AppSettings["prismic.token"];
			return await this.Get (accessToken);
		}
	}
}

