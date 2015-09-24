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
                tolls = input.routeType.tolls.ToString(),
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
                geometry = r.segments.Where(s => s.bounds != null).SelectMany(s => s.geometry).SelectMany(s => s)
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