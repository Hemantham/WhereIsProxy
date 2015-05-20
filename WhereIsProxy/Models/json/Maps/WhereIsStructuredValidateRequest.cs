using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhereIsProxy.Models.json.Maps
{

    public class ValidateStreet
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class ValidateAddress
    {
        public string number { get; set; }
        public ValidateStreet street { get; set; }
        public string suburb { get; set; }
        public string postcode { get; set; }
        public string state { get; set; }
    }

    public class WhereIsStructuredValidateRequest
    {
        public ValidateAddress address { get; set; }
    }
}