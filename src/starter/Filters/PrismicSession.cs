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
			var timeoutSetting = WebConfigurationManager.AppSettings ["prismic.session.timeout.minutes"];
			var sslSetting = WebConfigurationManager.AppSettings ["prismic.session.requireSSL"];
			this.timeoutMinutes = timeoutSetting != null
				? int.Parse (timeoutSetting)
				: TIMEOUT_MINS;
			this.requiresSsl = sslSetting != null
				? bool.Parse (sslSetting)
				: true;
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

		public string AccessToken
		{
			get 
			{ 
				var cookie = this.httpContext.Request.Cookies [ACCESS_TOKEN]; 
				return
					cookie == null ? null : cookie.Value;
			}
		}
	}
}

