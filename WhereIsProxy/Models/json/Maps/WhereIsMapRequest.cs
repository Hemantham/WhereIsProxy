using System.Collections.Generic;

namespace WhereIsProxy.Models.json.Maps
{
    public class Point
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class WhereIsMapRequest
    {
        public int height { get; set; }
        public int width { get; set; }
        public List<string> layers { get; set; }
        public Point centre { get; set; }
        public int zoom { get; set; }
        public List<MapData> mapData { get; set; }

        
    }
    public class WhereIsViewPortByZoomRequest
    {
        public int height { get; set; }
        public int width { get; set; }
        public Point centre { get; set; }
        public int zoom { get; set; }
    }

      public class WhereIsViewPortByPointRequest
    {
        public int height { get; set; }
        public int width { get; set; }
        public List<MapData> mapData { get; set; }
    }

    
    public interface IShape
    {
        
    }

    public class Circle : IShape
    {
        public Point centre { get; set; }
        public int radiusX { get; set; }
        public int radiusY { get; set; }
    }

    public class MapData
    {
        public string type { get; set; }
        public List<IShape> values { get; set; }
        public Style style { get; set; }
    }

    public class Style
    {
        public string fillColour { get; set; }
        public string strokeColour { get; set; }
        public string strokeWidth { get; set; }
        public string fillOpacity { get; set; }
        public string strokeOpacity { get; set; }
    }

    public class Marker : IShape
    {
        public int height { get; set; }
        public int width { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }
        public Point point { get; set; }
        public Icon icon { get; set; }
        public string url { get; set; }
    }

    public class Icon
    {
        public string colour { get; set; }
        public string colour1 { get; set; }
        public string colour2 { get; set; }
        public string text { get; set; }
        public string textColour { get; set; }
        public string type { get; set; }
    }

    public class Polyline : IShape
    {
        public List<Point> points { get; set; }

    }

}
