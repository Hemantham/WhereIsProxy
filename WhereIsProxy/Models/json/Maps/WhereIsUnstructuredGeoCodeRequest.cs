using System.Collections.Generic;

namespace WhereIsProxy.Models.json.Maps
{
    public class WhereIsUnstructuredGeoCodeRequest
    {
        public string query { get; set; }
        public IEnumerable<string> granularity { get; set; }
    }
}
