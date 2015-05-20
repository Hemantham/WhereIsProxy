using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhereIsProxy.Models.json.Maps
{
    //public class WhereIsStructuredValidateResponse
    //{
    //}

    //public class CentrePoint
    //{
    //    public double lon { get; set; }
    //    public double lat { get; set; }
    //}

    //public class StreetPoint
    //{
    //    public double lon { get; set; }
    //    public double lat { get; set; }
    //}

    public class Street
    {
        public string name { get; set; }
        public string type { get; set; }
        public string display { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class ValidatedAddress
    {
        public string number { get; set; }
        public Street street { get; set; }
        public string property { get; set; }
        public string suburb { get; set; }
        public List<string> suburbVanities { get; set; }
        public string postcode { get; set; }
        public List<string> regions { get; set; }
        public string state { get; set; }
        public string display { get; set; }
    }

    //public class Centre
    //{
    //    public double lon { get; set; }
    //    public double lat { get; set; }
    //}

   

    //public class Geometry
    //{
    //    public Centre centre { get; set; }
    //    public Street2 street { get; set; }
    //}

    public class GeocodedAddress
    {
        public string id { get; set; }
        public Point centrePoint { get; set; }
        public Point streetPoint { get; set; }
        public bool approximated { get; set; }
        public string granularity { get; set; }
        public ValidatedAddress address { get; set; }
        public Geometry geometry { get; set; }
    }

    public class ValidateResult
    {
        public List<string> adjustments { get; set; }
        public GeocodedAddress geocodedAddress { get; set; }
    }

    //public class Pagination
    //{
    //    public int start { get; set; }
    //    public int total { get; set; }
    //}

    public class WhereIsStructuredValidateResponse
    {
        public List<ValidateResult> results { get; set; }
        public Pagination pagination { get; set; }
    }
}