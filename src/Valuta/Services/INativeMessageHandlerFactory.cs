using System;
using System.Net.Http;

namespace Valuta.Services
{
    public interface INativeMessageHandlerFactory
    {
		HttpMessageHandler GetNativeHandler();
    }
}
