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
using Valuta.Models;
using Realms;
using DryIoc;
using System.Runtime.Serialization;

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

        
		public async Task RefreshRatesAsync(Realm realm)
		{
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
                    var json = await _client.RefreshRatesAsync(realm);
                    var details = JObject.Parse(json);
                    IList<JToken> results = details["rates"].Children().ToList();
                
                foreach (JProperty prop in results)
    			{
					float calc = 1 / (float)prop.First;

                    if(prop.Name != "BTC")
					{
						var cur = new Currency() { BaseCur = prop.Name };

                        if(calc < 0.1)
						{
							cur.CurrentValue = Math.Round(calc * 1000, 4);
                            cur.CompareValue = 1000;
						}

                        else if(calc < 1)
						{
							cur.CurrentValue = Math.Round(calc * 100, 4);
							cur.CompareValue = 100;
						}
						else
						{
							cur.CurrentValue = Math.Round(calc, 4);
							cur.CompareValue = 1;
						}

						realm.Add(cur, update: true);					}
    			}

                });
            
            return;
		}

		public async Task GetDailyTrendAsync(Realm realm)
		{
			DateTime dateTime = DateTime.Now;
			string yesterday = dateTime.AddDays(-1).ToString("yyyy-MM-dd");

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
				    var json = await _client.GetDailyTrendAsync(yesterday, realm);
                    var details = JObject.Parse(json);
                    IList<JToken> results = details["rates"].Children().ToList();

                    foreach (JProperty prop in results)
                    {
                        float calc = 1 / (float)prop.First;
                    

                        if (prop.Name != "BTC")
                        {
							var cur = realm.Find<Currency>(prop.Name);
                        
                            if (calc < 0.1)
                            {
                                cur.YesterdayValue = Math.Round(calc * 1000, 4);
                            }

                            else if (calc < 1)
                            {
                                cur.YesterdayValue = Math.Round(calc * 100, 4);
                            }
                            else
                            {
                                cur.YesterdayValue = Math.Round(calc, 4);
						    }

                            realm.Add(cur, update: true);
                        }
                    }

                });

			return;
		}
    }
}
