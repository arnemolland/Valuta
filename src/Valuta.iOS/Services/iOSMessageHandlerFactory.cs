using System;
using Valuta.Services;
using System.Net.Http;
namespace Valuta.iOS.Services
{
	public class iOSMessageHandlerFactory : INativeMessageHandlerFactory
    {
		public HttpMessageHandler GetNativeHandler()
		{
			return new NSUrlSessionHandler();
		}
    }
}
