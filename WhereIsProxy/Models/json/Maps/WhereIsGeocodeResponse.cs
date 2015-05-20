using System.Collections.Generic;

namespace WhereIsProxy.Models.json.Maps
{

    //public class Street
    //{
    //    //public string name { get; set; }
    //    //public string type { get; set; }
    //    //public string display { get; set; }


    //    public double lon { get; set; }
    //    public double lat { get; set; }
    //}

    public class Address
    {

        public string number { get; set; }
        public Street street { get; set; }
             
        public string suburb { get; set; }
        public string postcode { get; set; }
        public List<string> regions { get; set; }
        public string state { get; set; }
        public string display { get; set; }
    }

    public class Geometry
    {
        public Point Point { get; set; }
        public Point street { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public Point centrePoint { get; set; }
        public Point streetPoint { get; set; }
        public bool approximated { get; set; }
        public string granularity { get; set; }
        public Address address { get; set; }
        public Geometry geometry { get; set; }
    }

    public class Pagination
    {
        public int start { get; set; }
        public int total { get; set; }
    }

    public class WhereIsGeocodeResponse
    {
        public string keywords { get; set; }
        public List<Result> results { get; set; }
        public Pagination pagination { get; set; }
    }
}
