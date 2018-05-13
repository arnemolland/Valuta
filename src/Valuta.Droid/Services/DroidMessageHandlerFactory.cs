using System;
using Valuta.Services;
using Xamarin.Forms.Internals;
using System.Net.Http;
using Xamarin.Android.Net;

namespace Valuta.Droid.Services
{
    [Preserve]	
	public class DroidMessageHandlerFactory : INativeMessageHandlerFactory
    {
		public HttpMessageHandler GetNativeHandler()
		{
			return new AndroidClientHandler();
		}
    }
}
