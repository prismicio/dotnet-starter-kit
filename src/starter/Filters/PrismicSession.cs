using System;
using System.Web;
using System.Web.Configuration;
using Microsoft.FSharp.Core;
using prismic.extensions;

namespace prismic.mvc.starter
{
	public class PrismicSession
	{
		const int TIMEOUT_MINS = 30;
		const string ACCESS_TOKEN = "ACCESS_TOKEN";
		readonly HttpContextBase httpContext;
		readonly int timeoutMinutes;
		readonly bool requiresSsl;

		public PrismicSession (HttpContextBase httpContext)
		{
			this.httpContext = httpContext;
			this.timeoutMinutes = 
				WebConfigurationManager.AppSettings.TryGet ("prismic.session.timeout.minutes")
					.Map(mins => int.Parse(mins))
					.GetOrElse<int> (TIMEOUT_MINS);
			this.requiresSsl =
				WebConfigurationManager.AppSettings.TryGet ("prismic.session.requireSSL")
					.Map(requires => bool.Parse(requires))
					.GetOrElse<bool> (true);
		}

		public void Clear()
		{
			if (this.httpContext.Response.Cookies[ACCESS_TOKEN] != null)
			{
				this.httpContext.Response.Cookies[ACCESS_TOKEN].Value = null;
				this.httpContext.Response.Cookies[ACCESS_TOKEN].Expires = DateTime.Now.AddDays(-1);
			}

		}

		public void Open(string accessToken)
		{	
			var expires = DateTime.Now.AddMinutes(this.timeoutMinutes);
			var cookie = new HttpCookie(ACCESS_TOKEN);
			cookie.Value = accessToken;
			cookie.HttpOnly = true;
			cookie.Secure = this.requiresSsl; 
			cookie.Expires = expires;
			this.httpContext.Response.Cookies.Add(cookie);
		}

		public FSharpOption<string> AccessToken
		{
			get 
			{ 
				var cookie = this.httpContext.Request.Cookies [ACCESS_TOKEN]; 
				return
					cookie == null ? FSharpOption<string>.None : FSharpOption<string>.Some (cookie.Value); 
			}
		}
	}
}

