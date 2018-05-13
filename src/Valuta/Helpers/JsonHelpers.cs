using System;
using System.Runtime.Serialization;

namespace Valuta.Helpers
{
	[DataContract]
    public class CurrencyHelpers
    {
        [DataMember(Name = "base")]
        public string @base { get; set; }
        [DataMember(Name = "date")]
        public string date { get; set; }
        [DataMember(Name = "rates")]
        public Rates rates { get; set; }
    }

    [DataContract]
    public class Rates
    {
        [DataMember(Name = "NOK")]
        public string NOK { get; set; }
    }
}
