using System;
using Realms;
using PropertyChanged;
using Realms.Exceptions;
using Valuta.Helpers;

namespace Valuta.Models
{
	[AddINotifyPropertyChangedInterface]
	public class Currency : RealmObject
    {
        [PrimaryKey]
		public string BaseCur { get; set; }
		public string QuoteCur { get; set; }
		public string FullName { get { return GetFullName(); }}
		public double CurrentValue { get; set; }

		public double YesterdayValue { get; set; }
		public double MonthlyValue { get; set; }
		public double LastMonthValue { get; set; }
		public double YearlyValue { get; set; }
		public double LastYearValue { get; set; }
        
		public double DailyTrend { get { return ((CurrentValue - YesterdayValue) / Math.Abs(YesterdayValue)) * 100; }}
		public double MonthlyTrend { get { return ((MonthlyValue - LastMonthValue) / Math.Abs(LastMonthValue)) * 100; }}
		public double YearlyTrend { get { return ((YearlyValue - LastYearValue) / Math.Abs(LastYearValue)) * 100; }}
              
		public string CurrentTrendIcon { get { return GetTrendIcon(); } }
		public double CompareValue { get; set; }
		public string InfoLabel { get { return String.Format("{0} {1} I NOK", CompareValue, BaseCur); }}

		public string GetTrendIcon()
		{
			switch (Settings.SelectedTrend)
			{
				case "A":
					if (YearlyTrend > 0)
						return "TrendUp.svg";
					else if (YearlyTrend == 0)
						return "TrendStable.svg";
					else
						return "TrendDown.svg";

				case "M":
					if (MonthlyTrend > 0)
						return "TrendUp.svg";
					else if (MonthlyTrend == 0)
						return "TrendStable.svg";
					else
						return "TrendDown.svg";

				case "B":
					if (DailyTrend > 0)
						return "TrendUp.svg";
					else if (DailyTrend == 0)
						return "TrendStable.svg";
					else
						return "TrendDown.svg";

				default: return "TrendStable.svg";
			}
		}

        public string GetFullName()
		{
			switch(BaseCur)
			{
				case"NOK":
					return "Norske kroner";

				case "USD":
                    return "Amerikanske dollar";

				case "EUR":
                    return "Euro";

				case "GBP":
                    return "Britiske pund";

				case "SEK":
                    return "Svenske kroner";

				case "DKK":
                    return "Danske kroner";

				case "CAD":
                    return "Canadiske dollar";

				case "AUD":
                    return "Australske dollar";

				case "JPY":
                    return "Japanske yen";

				case "THB":
                    return "Thailandske baht";

				case "TRY":
                    return "Tyrkisk lira";

				case "HKD":
                    return "Hong Kong Dollar";

				case "SGD":
                    return "Singaporske Dollar";

				case "RUB":
                    return "Russike rubler";

				case "PLN":
                    return "Polske zloty";

				case "CHF":
                    return "Sveitsiske franc";

				case "AFN":
                    return "Afghansk afghani";

				case "DZD":
                    return "Algerisk dinar";

				case "ANG":
                    return "Antilliansk gylden";

				case "BTC":
                    return "Bitcoin";

				case "BRL":
                    return "Brasilske real";

				case "BGN":
                    return "Bulgarsk lev";

				case "CLP":
                    return "Chilensk peso";

				case "ISK":
                    return "Islandsk krone";

				case "MXN":
                    return "Mexikansk peso";

				case "VND":
                    return "Vietnamesisk dong";

				case "CNY":
                    return "Kinesisk renminbi";

				default:
					return "";
			}
		}
	}
}
