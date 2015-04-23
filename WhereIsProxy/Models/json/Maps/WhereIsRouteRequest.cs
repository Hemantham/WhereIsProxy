using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhereIsProxy.Models.json.Maps
{
    public class WhereIsRouteRequest
    {
        public string mode { get; set; }
        public string method { get; set; }
        public string tolls { get; set; }
        public string imageFormat { get; set; }
        public List<Waypoint> waypoints { get; set; }
        public List<Point> avoidPoints { get; set; }
    }

    public class Waypoint
    {
        public Point streetPoint { get; set; }
        public Point centrePoint { get; set; }
        public Street street { get; set; }
    }
}