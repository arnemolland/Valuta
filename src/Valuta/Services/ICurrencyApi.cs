using System;
using Refit;
using System.Data;
using System.Threading.Tasks;
using Valuta.Models;
using System.Collections.Generic;

namespace Valuta.Services
{
	public interface ICurrencyApi
	{
		[Get("/api/latest?access_key=be829987b4e30a459c9aea6199d39358&base={ticker}&symbols=NOK")]
		Task<string> GetRateAsync(string ticker);
        
    }
}
