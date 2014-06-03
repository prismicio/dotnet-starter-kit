using System;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Runtime.Caching;
using prismic;
using Microsoft.FSharp.Core;

namespace prismic.mvc.starter
{
	/// <summary>
	/// Simple cache built using the standard .Net MemoryCache
	/// used by prismic kit to store Api Responses
	/// </summary>
	public class PrismicMemoryCache : prismic.ApiInfra.ICache<Api.Response>
	{
		static readonly Lazy<PrismicMemoryCache> instance = new Lazy<PrismicMemoryCache>(() => new PrismicMemoryCache());
		readonly MemoryCache cache;
		private PrismicMemoryCache()
		{
			this.cache = MemoryCache.Default; // anyway, return the default MemoryCache
		}
		public static PrismicMemoryCache Instance
		{
			get { return instance.Value; }
		}
		public void Set(string key, Api.Response response, DateTimeOffset expirationOffset)
		{
			cache.Set (key, response, expirationOffset);
		}
		public FSharpOption<Api.Response> Get(string key)
		{
			var value = cache.Get (key);
			return value == null 
				? FSharpOption<Api.Response>.None
				: FSharpOption<Api.Response>.Some ((Api.Response)value);
		}
	}
		
}

