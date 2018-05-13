using System;
using System.Net.Http;
using System.Threading.Tasks;
using Valuta.Helpers;
using Polly;
using Refit;
using System.Net;
using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Data;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using Microsoft.AppCenter.Distribute;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using System.Linq;

namespace Valuta.Services
{
    public class RestClient
    {
		private ICurrencyApi _client;

		private static INativeMessageHandlerFactory _factory;

		public static void Init (INativeMessageHandlerFactory factory)
		{
			_factory = factory;
		}

		public RestClient()
		{
			var nativeClient = new HttpClient(_factory.GetNativeHandler())
			{
				BaseAddress = new System.Uri(AppConstants.ApiBaseAdress)
			};

			_client = Refit.RestService.For<ICurrencyApi>(nativeClient);
		}
        
        public async Task<double> GetRateAsync(string ticker)
		{
			double test = 0;
			await Policy
				.Handle<ApiException>(ex => ex.StatusCode != HttpStatusCode.NotFound)
				.WaitAndRetryAsync
				(
					retryCount: 5,
					sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
				)
				.ExecuteAsync(async () =>
			{
    			Debug.WriteLine("Trying service call...");
    			var json = await _client.GetRateAsync(ticker);
				var details = JObject.Parse(json);
				IList<JToken> results = details["rates"].Children().ToList();
				float temp = results[0].ToObject<float>();
				test = (double)temp;

                
                                               
			});
			return Math.Round(test, 4);
		}
    }
}
