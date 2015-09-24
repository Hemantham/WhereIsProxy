using System.IO;
using System.Xml;
using System.Xml.Serialization;
using RestSharp;
using WhereIsProxy.Models;

namespace WhereIsProxy.MappingProviders
{
    public class WhereIsResponseGenerator
    {
        public static string Generate<TIN, TOUT, TMapper>(SOAPEnvelopeBase inputEnvelop )
            where TIN : class , new()
            where TOUT : class , new()
            where TMapper : WhereIsMappingProvider<TIN,TOUT> , new ()
        {
            string responseString;

            TMapper mapper = new TMapper();

            var input = (SOAPEnvelope<TIN>)inputEnvelop;

            var type = input.body.bodyvalue.GetType();

            var xOverrides = new XmlAttributeOverrides();

            var valuetype = type;

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();

            nameSpaces.Add("ns1", "http://ems.whereis.com/");


            XmlElementAttribute myElementAttribute = new XmlElementAttribute();

            myElementAttribute.ElementName = valuetype.Name + "Response";

            myElementAttribute.Namespace = "http://ems.whereis.com/";

            XmlAttributes myAttributes = new XmlAttributes();

            myAttributes.XmlElements.Add(myElementAttribute);

            xOverrides.Add(typeof(MessageBody<TOUT>), "bodyvalue", myAttributes);

            var soapserializer = new XmlSerializer(typeof(SOAPEnvelope<TOUT>), xOverrides);

            var soapEnvilope = new SOAPEnvelope<TOUT>
            {
                body = new MessageBody<TOUT>()
            };
           
            soapEnvilope.body.bodyvalue = mapper.Map(input.body.bodyvalue);

            var settings = new XmlWriterSettings();

            settings.OmitXmlDeclaration = true;

            using (var writer = new StringWriter())

            using (var xmlwriter = XmlWriter.Create(writer, settings))
            {
                soapserializer.Serialize(xmlwriter, soapEnvilope, nameSpaces);

                responseString = writer.ToString();
            }

            return responseString;
        }
    }
}