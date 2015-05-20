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
    public class WhereIsMapProvider : WhereIsMappingProvider<getMapByCentre, getMapByCentreResponse>
    {
        public WhereIsMapProvider()
            : base()
        {

        }

        public WhereIsMapProvider(IRestClient client)
            : base(client)
        {

        }
        public override getMapByCentreResponse Map(getMapByCentre getMapByCentre)
        {
            var mapMarkers = getMapByCentre.markers.Select(m => new MapData
            {
                type = "marker"
                ,
                values = new List<Marker>{{new Marker
                {
                    height = m.height,
                    icon = new Icon
                    {
                        colour =  m.builtIn.type.ToString() == "poi" ? "green" : null, 
                        text = m.builtIn.text == "cross_hair" ? "cross_hairs" :m.builtIn.text  ,
                        type = m.builtIn.type.ToString(),
                        
                    },
                    point = new Point
                    {
                        lat = m.point.latitude ,
                        lon = m.point.longitude ,
                    }
                    
                }}}.ToList<IShape>()
            }).ToList();

            if (getMapByCentre.routes != null)
            {
                getMapByCentre.routes.SelectMany(r => r.geometry);
                var mapRout = new MapData
                {
                    type = "polyline",
                    values = new List<Polyline>
                    {
                        {
                            new Polyline
                            {
                                points = getMapByCentre.routes
                                    .SelectMany(r => r.geometry)
                                    .Select(g => new Point
                                    {
                                        lat = g.latitude,
                                        lon = g.longitude,
                                    }).ToList(),
                            }
                        }
                    }.ToList<IShape>(),
                    style = new Style
                    {
                        strokeColour = "#0E4B05",
                        strokeOpacity = "0.5",
                        strokeWidth = "7"

                    }
                };

                mapMarkers.Add(mapRout);
            }
            var whereIsMapRequest = new WhereIsMapRequest

            {
                centre = new Point { lat = getMapByCentre.centre.latitude, lon = getMapByCentre.centre.longitude },
                height = getMapByCentre.height,
                width = getMapByCentre.width,
                zoom = getMapByCentre.zoom,
                layers = new List<string> { "STREET" },
                mapData = mapMarkers
            };

            var mapRequest = getRestRequest(whereIsMapRequest, "service/map/by_zoom", getMapByCentre.authorisation.token, getMapByCentre.authorisation.password);

            var mapResponse = JsonConvert.DeserializeObject<WhereIsMapResponse>(client.Execute(mapRequest).Content);

            var soapresponse = new getMapByCentreResponse
            {
                @return = new soapMapArea
                {
                    encodedGraphic = ImageHelper.ConvertImageUrlToBase64(mapResponse.url),
                    boundingBox = new soapBox
                    {
                        botRight = new soapPoint { latitude = mapResponse.viewPort.bounds.bottom, longitude = mapResponse.viewPort.bounds.right },
                        topLeft = new soapPoint { latitude = mapResponse.viewPort.bounds.top, longitude = mapResponse.viewPort.bounds.left },
                    }
                    ,
                    zoom = mapResponse.viewPort.zoom
                }
            };

            return soapresponse;
        }

    }
}