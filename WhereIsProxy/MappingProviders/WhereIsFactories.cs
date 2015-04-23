using System.Xml.Serialization;
using WhereIsProxy.Models;
using WhereIsProxy.WhereIsSchemaReference;

namespace WhereIsProxy.MappingProviders
{
    public class SerializerFactory
    {
        private static XmlSerializer getXmlSerializer<T>()
        {
            var xmlOverrides = new XmlAttributeOverrides();

            XmlElementAttribute myElementAttribute = new XmlElementAttribute();

            myElementAttribute.ElementName = typeof(T).Name;

            myElementAttribute.Namespace = "http://ems.whereis.com/";

            XmlAttributes myAttributes = new XmlAttributes();

            myAttributes.XmlElements.Add(myElementAttribute);

            xmlOverrides.Add(typeof(MessageBody<T>), "bodyvalue", myAttributes);

            var soapserializer = new XmlSerializer(typeof(SOAPEnvelope<T>), xmlOverrides);

            return soapserializer;
        }
       
        public static XmlSerializer Get(string valuetype)
        {
            if (valuetype == typeof(geocode).Name)
            {
                return getXmlSerializer<geocode>();
            }
            else if (valuetype == typeof(getMapByCentre).Name)
            {
                return getXmlSerializer<getMapByCentre>();
            }
            else if (valuetype == typeof(route).Name)
            {
                return getXmlSerializer<route>();
            }
            else if (valuetype == typeof(getViewportForPoints).Name)
            {
                return getXmlSerializer<getViewportForPoints>();
            }

            return getXmlSerializer<geocode>();
        }
    }

    public class ResponseXmlFactory
    {
        private static XmlSerializer getXmlSerializer<T>()
        {
            var xmlOverrides = new XmlAttributeOverrides();

            XmlElementAttribute myElementAttribute = new XmlElementAttribute();

            myElementAttribute.ElementName = typeof(T).Name;

            myElementAttribute.Namespace = "http://ems.whereis.com/";

            XmlAttributes myAttributes = new XmlAttributes();

            myAttributes.XmlElements.Add(myElementAttribute);

            xmlOverrides.Add(typeof(MessageBody<T>), "bodyvalue", myAttributes);

            var soapserializer = new XmlSerializer(typeof(SOAPEnvelope<T>), xmlOverrides);

            return soapserializer;
        }

        public static string Get(SOAPEnvelopeBase inputEnvelop)
        {
            var responseString = string.Empty;

            if (inputEnvelop is SOAPEnvelope<geocode>)
            {
                responseString = WhereIsResponseGenerator.Generate<geocode, geocodeResponse, WhereIsGeoCodeProvider>(inputEnvelop);
            }
            else if (inputEnvelop is SOAPEnvelope<getMapByCentre>)
            {
                responseString = WhereIsResponseGenerator.Generate<getMapByCentre, getMapByCentreResponse, WhereIsMapProvider>(inputEnvelop);
            }
            else if (inputEnvelop is SOAPEnvelope<route>)
            {
                responseString = WhereIsResponseGenerator.Generate<route, routeResponse, WhereIsRouteProvider>(inputEnvelop);
            }
            else if (inputEnvelop is SOAPEnvelope<getViewportForPoints>)
            {
                responseString = WhereIsResponseGenerator.Generate<getViewportForPoints, getViewportForPointsResponse, WhereIsViewportProvider>(inputEnvelop);
            }

            return responseString;
        }
    }
}