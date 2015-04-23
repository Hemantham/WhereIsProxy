using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using WhereIsProxy.MappingProviders;
using WhereIsProxy.WhereIsSchemaReference;

namespace WhereIsProxy.Tests.MappingProviders
{
    [TestClass]
    public class MappingProviderTest
    {
        private Mock<IRestClient> restClientMock;
        private WhereIsGeoCodeProvider geoCodeProvider;
        private WhereIsMapProvider whereIsMapProvider;
        private WhereIsRouteProvider whereIsRouteProvider;
        private WhereIsProxy.WhereIsSchemaReference.geocode geocodeinput;
        private WhereIsProxy.WhereIsSchemaReference.getMapByCentre getMapByCentreinput;
        private WhereIsProxy.WhereIsSchemaReference.geocodeResponse geocodeexpectedOutput;
        private RestResponse geoCodeRestResponse = new RestResponse();

        [TestInitialize]
        public void Setup()
        {
            Setup_geocode();
            Setup_map();
        }

        public void Setup_geocode()
        {
           

            geoCodeRestResponse.Content =

                "{\"keywords\":\"\",\"results\":[{\"id\":\"suburb-11467\",\"centrePoint\":{\"lon\":144.3593453,\"lat\":-38.1480041},\"streetPoint\":{\"lon\":144.3593453,\"lat\":-38.1480041},\"bounds\":{\"left\":144.35088164,\"top\":-38.1322695508,\"right\":144.374881415,\"bottom\":-38.1639876595},\"approximated\":false,\"granularity\":\"SUBURB\",\"address\":{\"suburb\":\"GEELONG\",\"postcode\":\"3220\",\"regions\":[\"GREATER GEELONG\",\"GREATER GEELONG COUNCIL\"],\"state\":\"VIC\",\"display\":\"GEELONG, VIC 3220\"},\"geometry\":{\"bounds\":{\"left\":144.35088164,\"top\":-38.1322695508,\"right\":144.374881415,\"bottom\":-38.1639876595},\"centre\":{\"lon\":144.3593453,\"lat\":-38.1480041},\"street\":{\"lon\":144.3593453,\"lat\":-38.1480041}}}],\"pagination\":{\"start\":0,\"total\":1}}";

            geocodeinput = new WhereIsProxy.WhereIsSchemaReference.geocode
            {
                address = new WhereIsProxy.WhereIsSchemaReference.soapAddress
                    {

                        state = "VIC",
                        street = "5 Homestead Street",
                        suburb = "GEELONG 3220",

                    },

                authorisation = new WhereIsProxy.WhereIsSchemaReference.soapAuth
                    {
                        password = "icon?2765121??global!",
                        token = "9219514437938748416",
                    },

            };



            geocodeexpectedOutput = new WhereIsProxy.WhereIsSchemaReference.geocodeResponse
                {
                    geocodeResponse1 = new WhereIsProxy.WhereIsSchemaReference.soapGeocodeResult[]
	                {
		                new WhereIsProxy.WhereIsSchemaReference.soapGeocodeResult
		                {
			                accuracy = 100.0f,
			                featureSpecified = false,
			                geocodedAddress = new WhereIsProxy.WhereIsSchemaReference.soapGeocodedAddress
			                {
				              coordinates =    new WhereIsProxy.WhereIsSchemaReference.soapPoint
				                {
					                latitude = -38.1480041,
					                longitude = 144.3593453,
				                },
				                number = "",
				                postcode = "3220",
				                state = "VIC",
				               
				              street  =  new WhereIsProxy.WhereIsSchemaReference.soapStreet
				                {
					                directionalPrefix = null,
					                directionalSuffix = null,
					                fullName = "",
					                name = "",
					                type = null,
					             
				                },
				              streetCoordinates   = new WhereIsProxy.WhereIsSchemaReference.soapPoint
				                {
					                latitude = -38.1480041,
					                longitude = 144.3593453,
					               
				                },
				                suburb = "GEELONG",
			                },
		                }
	                }
                };

          

        }
        
        public void Setup_map()
        {
            
            getMapByCentreinput = new WhereIsProxy.WhereIsSchemaReference.getMapByCentre
            {
                authorisation = new WhereIsProxy.WhereIsSchemaReference.soapAuth
                {
                    password = "icon?2765121??global!",
                    token = "9219514437938748416",

                },

                centre = new WhereIsProxy.WhereIsSchemaReference.soapPoint
                {
                    latitude = -38.146491,
                    longitude = 144.363914,
                },

                height = 532,
                imageFormat = null,
                layer = "map",
                markers = new WhereIsProxy.WhereIsSchemaReference.soapMarker[]
                {
                    new WhereIsProxy.WhereIsSchemaReference.soapMarker
                    {
                        builtIn = new WhereIsProxy.WhereIsSchemaReference.soapIcon
                        {
                            text = "cross_hair",
                            textColour = null,
                            type = soapIconType.standard,
                            typeSpecified = true
                        },

                        height = 0,
                        offsetX = 0,
                        offsetY = 0,
                        point = new WhereIsProxy.WhereIsSchemaReference.soapPoint
                        {
                            latitude = -38.146491,
                            longitude = 144.363914,
                        },

                        width = 0,
                    }
                },
            };
            
         
          

        }


        [TestMethod]
        public void WhereIsGeoCodeProvider_Test()
        {
            restClientMock = new Mock<IRestClient>();
            var restgeocodeResoponse = new RestResponse();
            restgeocodeResoponse.Content =
                "{\"keywords\":\"\",\"results\":[{\"id\":\"suburb-11467\",\"centrePoint\":{\"lon\":144.3593453,\"lat\":-38.1480041},\"streetPoint\":{\"lon\":144.3593453,\"lat\":-38.1480041},\"bounds\":{\"left\":144.35088164,\"top\":-38.1322695508,\"right\":144.374881415,\"bottom\":-38.1639876595},\"approximated\":false,\"granularity\":\"SUBURB\",\"address\":{\"suburb\":\"GEELONG\",\"postcode\":\"3220\",\"regions\":[\"GREATER GEELONG\",\"GREATER GEELONG COUNCIL\"],\"state\":\"VIC\",\"display\":\"GEELONG, VIC 3220\"},\"geometry\":{\"bounds\":{\"left\":144.35088164,\"top\":-38.1322695508,\"right\":144.374881415,\"bottom\":-38.1639876595},\"centre\":{\"lon\":144.3593453,\"lat\":-38.1480041},\"street\":{\"lon\":144.3593453,\"lat\":-38.1480041}}}],\"pagination\":{\"start\":0,\"total\":1}}";

            restClientMock.Setup(s => s.Execute(It.IsAny<IRestRequest>())).Returns(restgeocodeResoponse);

            geoCodeProvider = new WhereIsGeoCodeProvider(restClientMock.Object);

           

            var soap = geoCodeProvider.Map(geocodeinput);
            Assert.AreEqual(geocodeexpectedOutput.geocodeResponse1.First().geocodedAddress.postcode , soap.geocodeResponse1.First().geocodedAddress.postcode);
            Assert.AreEqual(geocodeexpectedOutput.geocodeResponse1.First().geocodedAddress.state, soap.geocodeResponse1.First().geocodedAddress.state);
            Assert.AreEqual(geocodeexpectedOutput.geocodeResponse1.First().geocodedAddress.coordinates.latitude, soap.geocodeResponse1.First().geocodedAddress.coordinates.latitude);
            Assert.AreEqual(geocodeexpectedOutput.geocodeResponse1.First().geocodedAddress.coordinates.longitude, soap.geocodeResponse1.First().geocodedAddress.coordinates.longitude);
        }

        [TestMethod]
        public void WhereIsMapProvider_Test()
        {
            restClientMock = new Mock<IRestClient>();

            var testResponse = new RestResponse();

            testResponse.Content =
                "{\"url\":\"http://api.ems.sensis.com.au/v2/service/map/-799088810\",\"viewPort\":{\"bounds\":{\"left\":144.349924,\"top\":-38.137513,\"right\":144.377904,\"bottom\":-38.155468},\"centre\":{\"lon\":144.363914,\"lat\":-38.146491},\"resolution\":4.77731426715850826525411321199499070644378662109375,\"x_distance\":2445.502981,\"y_distance\":1995.164887,\"zoom\":13}}";
           
            restClientMock.Setup(s => s.Execute(It.IsAny<IRestRequest>())).Returns(testResponse);

            whereIsMapProvider = new WhereIsMapProvider(restClientMock.Object);

            var soap = whereIsMapProvider.Map(getMapByCentreinput);
            Assert.IsTrue( !string.IsNullOrEmpty( soap.@return.encodedGraphic));
            Assert.AreEqual(-38.155468, soap.@return.boundingBox.botRight.latitude);
        }
    }
}
