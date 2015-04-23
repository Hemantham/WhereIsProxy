namespace WhereIsProxy.Models.json.Maps
{
        public class Bounds
        {
            public double left { get; set; }
            public double top { get; set; }
            public double right { get; set; }
            public double bottom { get; set; }
        }

        public class WhereIsViewportResponse
        {
            public Bounds bounds { get; set; }
            public Point centre { get; set; }
            public double resolution { get; set; }
            public double x_distance { get; set; }
            public double y_distance { get; set; }
            public int zoom { get; set; }
        }

        public class ViewPort
        {
            public Bounds bounds { get; set; }
            public Point centre { get; set; }
            public double resolution { get; set; }
            public double x_distance { get; set; }
            public double y_distance { get; set; }
            public int zoom { get; set; }
        }
        public class WhereIsMapResponse
        {
            public string url { get; set; }
            public ViewPort viewPort { get; set; }
        }
}
