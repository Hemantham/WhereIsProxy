using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using WhereIsProxy.Images;
using WhereIsProxy.Models.json.Maps;
using WhereIsProxy.WhereIsSchemaReference;
using System.Text.RegularExpressions;
using System;

namespace WhereIsProxy.MappingProviders
{
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

            var request = getRestRequest(whereIsUnstructuredGeoCodeRequest, "service/geocode/unstructured", input.authorisation.token, input.authorisation.password);

            // execute the request
            var restResponse = JsonConvert.DeserializeObject<WhereIsGeocodeResponse>(client.Execute(request).Content);

            //check is it is a n exact match
            if (restResponse.results.Any(r => r.granularity == "PROPERTY" || r.granularity == "STREET"))
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
            else // make a validation request and get similar results
            {

                string numberString = string.Empty;
                string streetName = string.Empty;

                if (input.address.street != null)
                {
                    var regexNumber = new Regex("^[0-9]*$");
                    var regexalpha = new Regex("^[a-zA-Z]*$");
                    numberString = string.Join(" ", input.address.street.Split(' ', '.', '/', '\\', '-').Where(s => regexNumber.IsMatch(s))).Trim();
                    streetName = string.Join(" ", input.address.street.Split(' ', '.', '/', '\\', '-').Where(s => regexalpha.IsMatch(s))).Trim();
                }

                var whereIsStructuredValidateRequest = new WhereIsStructuredValidateRequest
                {
                    address = new ValidateAddress
                    {
                        number = string.IsNullOrEmpty(numberString) ? null : numberString,

                        street = new ValidateStreet
                        {
                            name = string.IsNullOrEmpty(streetName) ? null : streetName
                        },

                        state = string.IsNullOrEmpty(input.address.state) ? null : input.address.state

                    }

                };
                request = getRestRequest(whereIsStructuredValidateRequest, "service/validation/structured", input.authorisation.token, input.authorisation.password);

                // execute the request
                var valiadteRestResponse = JsonConvert.DeserializeObject<WhereIsStructuredValidateResponse>(client.Execute(request).Content);
                Random random = new Random();
               
                var results = valiadteRestResponse.results.Select(r => new soapGeocodeResult
                {
                    accuracy = random.Next(500, 600) / 10
                    ,
                    geocodedAddress = new soapGeocodedAddress
                    {
                        number = r.geocodedAddress.address.number ?? string.Empty,
                        postcode = r.geocodedAddress.address.postcode,
                        state = r.geocodedAddress.address.state,
                        street = new soapStreet
                        {

                            fullName = r.geocodedAddress.address.street != null ? r.geocodedAddress.address.street.display : string.Empty,
                            name = r.geocodedAddress.address.street != null ? r.geocodedAddress.address.street.name : string.Empty,
                        },
                        suburb = r.geocodedAddress.address.suburb,
                        coordinates = new soapPoint
                        {
                            latitude = r.geocodedAddress.centrePoint.lat,
                            longitude = r.geocodedAddress.centrePoint.lon,
                        }
                     ,
                        streetCoordinates = new soapPoint
                        {
                            latitude = r.geocodedAddress.streetPoint.lat,
                            longitude = r.geocodedAddress.streetPoint.lon,
                        }
                    }
                }).ToArray();
                response = new geocodeResponse(results);

            }


            return response;

        }
    }
}