using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using WhereIsProxy.Images;
using WhereIsProxy.Models.json.Maps;
using WhereIsProxy.WhereIsSchemaReference;

namespace WhereIsProxy.MappingProviders
{
    public abstract class WhereIsMappingProvider<TIN, TOUT>
        where TIN : class, new()
        where TOUT : class, new()
    {
        protected static string WhereisUri = ConfigurationManager.AppSettings.Get("WhereisUri");

        protected IRestClient client;

        protected WhereIsMappingProvider()
            : this(new RestClient(WhereisUri))
        {
            
        }

        protected WhereIsMappingProvider(IRestClient client)
        {
            this.client = client;
        }

        protected static RestRequest getRestRequest<T>(T requestJson, string resource, string token, string password)
        {
           
            var request = new RestRequest(resource, Method.POST);
            request.AddHeader("X-Auth-Password", password);
            request.AddHeader("X-Auth-Token", token);
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestJson, Formatting.Indented),
                ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            return request;
        }


        public abstract TOUT Map(TIN input);
    }

    public class WhereIsGeoCodeProvider : WhereIsMappingProvider<geocode, geocodeResponse>
    {

        public WhereIsGeoCodeProvider()
            : base()
        {

        }

        public WhereIsGeoCodeProvider(IRestClient client)
            : base(client)
        {
            
        }

        public override geocodeResponse Map(geocode input)
        {
           
            geocodeResponse response = new geocodeResponse();
            var whereIsUnstructuredGeoCodeRequest = new WhereIsUnstructuredGeoCodeRequest
            {
                granularity = new List<string> { "PROPERTY" },
                query = string.Format("{0}, {1}, {2}", input.address.street, input.address.suburb, input.address.state)
            };

            var mapRequest = getRestRequest(whereIsUnstructuredGeoCodeRequest, "service/geocode/unstructured", input.authorisation.token, input.authorisation.password);

            // execute the request
            var restResponse = JsonConvert.DeserializeObject<WhereIsGeocodeResponse>(client.Execute(mapRequest).Content);

            if (restResponse.results.Any())
            {
                var results = restResponse.results.Select(r => new soapGeocodeResult
                {
                    accuracy = 100.0f
                    ,
                    geocodedAddress = new soapGeocodedAddress
                    {
                        number = r.address.number ?? string.Empty,
                        postcode = r.address.postcode,
                        state = r.address.state,
                        street = new soapStreet
                        {

                            fullName = r.address.street != null ? r.address.street.display : string.Empty,
                            name = r.address.street != null ? r.address.street.name : string.Empty,
                        },
                        suburb = r.address.suburb,
                        coordinates = new soapPoint
                        {
                            latitude = r.centrePoint.lat,
                            longitude = r.centrePoint.lon,
                        }
                     ,
                        streetCoordinates = new soapPoint
                        {
                            latitude = r.streetPoint.lat,
                            longitude = r.streetPoint.lon,
                        }
                    }
                }).ToArray();
                 response = new geocodeResponse(results);
                
            }

        
            return response;
           
        }
    }

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
               
                mapData.Add(  new MapData
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
                    centre = new soapPoint{ latitude =  mapResponse.centre.lat , longitude = mapResponse.centre.lon },
                    resolution = mapResponse.resolution,
                    xdistance = mapResponse.x_distance ,
                    ydistance = mapResponse.y_distance ,
                }
            };

            return soapresponse;
        }

    }

    public class WhereIsRouteProvider : WhereIsMappingProvider<route, routeResponse>
    {
         public WhereIsRouteProvider()
            : base()
        {

        }

         public WhereIsRouteProvider(IRestClient client)
            : base(client)
        {
            
        }

        /// <summary>
        /// not exactly correct as not taking in to considerations the earths curvature
        /// </summary>
        /// <param name="bottom"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        private Point getCenter(Bounds bounds)
        {
            if (bounds != null)
            return new Point
            {
                lat = (bounds.bottom + bounds.top) / 2,
                lon = (bounds.left + bounds.right) / 2
            };
            return new Point();
        }

        public override routeResponse Map(route input)
        {
            var requestBody = new WhereIsRouteRequest
            {
                mode = "VEHICLE",
                method = "FASTEST",
                tolls = "false",
                imageFormat = "PNG",
                waypoints = input.wayPoints.Select(w => new Waypoint
                {
                    streetPoint = new Point
                    {
                        lat = w.latitude,
                        lon = w.longitude,
                    }

                }).ToList(),
            };

            var mapRequest = getRestRequest(requestBody, "service/route", input.authorisation.token, input.authorisation.password);

            // execute the request
            var response = JsonConvert.DeserializeObject<WhereIsRouteResponse>(client.Execute(mapRequest).Content);

            // response.route.routes = 1
            var routes = response.route.routes.Select(r => new soapRoute
            {
                distance = (float)r.distance,
                duration = r.duration,
                geometry = r.segments.Where(s=>s.bounds != null).SelectMany(s => s.geometry).SelectMany(s => s)
                .Select(s => new soapPoint { longitude = s[0], latitude = s[1] }).ToArray(),
                segments = r.segments.Select(s =>
                {
                    var center = getCenter(s.bounds);
                    return new soapRouteSegment
                    {
                        centre = new soapPoint { latitude = center.lat, longitude = center.lon },
                        textInstruction = s.instruction.textualInstruction,
                    };
                }).ToArray()
            }).ToArray();

            return new routeResponse(routes);

        }
    }
}