
using System;
using prismic;

namespace prismic.mvc.starter
{
	public class PrismicLogger: ILogger
	{
		public PrismicLogger ()
		{
		}

		public void log(string level, string message) {
			System.Diagnostics.Debug.WriteLine(level + ": " + message);
		}
	}
}

