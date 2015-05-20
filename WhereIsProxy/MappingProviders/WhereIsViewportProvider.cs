using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using WhereIsProxy.Images;
using WhereIsProxy.Models.json.Maps;
using WhereIsProxy.WhereIsSchemaReference;
using System.Text.RegularExpressions;

namespace WhereIsProxy.MappingProviders
{

    public class WhereIsViewportProvider : WhereIsMappingProvider<getViewportForPoints, getViewportForPointsResponse>
    {
        public WhereIsViewportProvider()
            : base()
        {

        }

        public WhereIsViewportProvider(IRestClient client)
            : base(client)
        {

        }
        public override getViewportForPointsResponse Map(getViewportForPoints input)
        {
            var mapData = new List<MapData>();

            mapData.Add(new MapData
            {
                type = "polyline",
                values = new List<Polyline>
                    {
                        {
                            new Polyline
                            {
                                points = input.points.Select(p=> new Point
                                {
                                    lat = p.latitude,
                                    lon = p.longitude,
                                }).ToList()
                            }
                        }
                    }.ToList<IShape>(),
            });


            var whereIsMapRequest = new WhereIsViewPortByPointRequest
            {
                height = input.height,
                width = input.width,
                mapData = mapData
            };

            var mapRequest = getRestRequest(whereIsMapRequest, "service/viewport/by_geometry", input.authorisation.token, input.authorisation.password);

            var mapResponse = JsonConvert.DeserializeObject<WhereIsViewportResponse>(client.Execute(mapRequest).Content);

            var soapresponse = new getViewportForPointsResponse
            {
                @return = new soapViewport
                {
                    boundingBox = new soapBox
                    {
                        botRight = new soapPoint { latitude = mapResponse.bounds.bottom, longitude = mapResponse.bounds.right },
                        topLeft = new soapPoint { latitude = mapResponse.bounds.top, longitude = mapResponse.bounds.left },
                    }
                    ,
                    zoom = mapResponse.zoom
                    ,
                    centre = new soapPoint { latitude = mapResponse.centre.lat, longitude = mapResponse.centre.lon },
                    resolution = mapResponse.resolution,
                    xdistance = mapResponse.x_distance,
                    ydistance = mapResponse.y_distance,
                }
            };

            return soapresponse;
        }

    }
}