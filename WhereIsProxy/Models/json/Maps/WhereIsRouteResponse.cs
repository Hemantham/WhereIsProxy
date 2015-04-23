using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhereIsProxy.Models.json.Maps
{
    public class WhereIsRouteResponse
    {
        public Route route { get; set; }
    }
    public class Handle
    {
        public string id { get; set; }
        public string expiry { get; set; }
    }
    public class Manoeuvre
    {
        public string type { get; set; }
        public string feature { get; set; }
    }
    public class Instruction
    {
        public Manoeuvre manoeuvre { get; set; }
        public string street { get; set; }
        public string suburb { get; set; }
        public double destinationAngle { get; set; }
        public string bearing { get; set; }
        public string textualInstruction { get; set; }
        public double? originAngle { get; set; }
        public double? turnAngle { get; set; }
    }

    public class Segment
    {
        public int segmentId { get; set; }
        public string roadType { get; set; }
        public List<string> encodedGeometry { get; set; }
        public List<List<double[]>> geometry { get; set; }
        public Bounds bounds { get; set; }
        public int duration { get; set; }
        public Point startPoint { get; set; }
        public double distance { get; set; }
        public Instruction instruction { get; set; }
    }
    public class Route
    {
        public Point endPoint { get; set; }
        public Handle handle { get; set; }
        public List<Route> routes { get; set; }
        public Bounds bounds { get; set; }
        public int duration { get; set; }
        public Point startPoint { get; set; }
        public double distance { get; set; }
        public List<Segment> segments { get; set; }
    }
}