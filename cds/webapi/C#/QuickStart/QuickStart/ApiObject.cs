using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples
{
    public class Value
    {
        [JsonProperty("odata.type")]
        public string Type { get; set; }
        public string Name { set; get; }
        public int? AccountNumber { get; set; }
        public string AccountId { get; set; }
        //public string Description { get; set; }
        //public DateTime ReleaseDate { get; set; }
        //public DateTime? DiscontinuedDate { get; set; }
        //public int Rating { get; set; }
        //public double Price { get; set; }
    }

    public class OData
    {
        [JsonProperty("odata.metadata")]
        public string Metadata { get; set; }
        public List<Value> Value { get; set; }
    }
}