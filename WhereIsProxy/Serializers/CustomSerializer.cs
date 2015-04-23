using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using WhereIsProxy.MappingProviders;
using WhereIsProxy.Models;
using WhereIsProxy.WhereIsSchemaReference;

namespace WhereIsProxy.Serializers
{
    public class CustomXmlFormatter : XmlMediaTypeFormatter
    {

        private string getType(ref string soapString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(soapString);

            //Select the book node with the matching attribute value.
            XmlNode nodeToFind;
            XmlElement root = doc.DocumentElement;

            foreach (XmlNode node in doc)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    doc.RemoveChild(node);
                }
            }

            soapString = doc.OuterXml;

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

            // Selects all the title elements that have an attribute named lang
            var body = root.SelectSingleNode("//soap:Envelope/soap:Body", nsmgr).ChildNodes;

            if (body.Count > 0)
            {
                return body[0].Name;
            }
           
            return null; //todo other types
           
        }

         
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {

            var taskSource = new TaskCompletionSource<object>();

            readStream.Position = 0;

            var streamReader = new StreamReader(readStream);

            var inputString = streamReader.ReadToEnd();

            var valuetype = getType(ref inputString);

            using (TextReader reader = new StringReader(inputString))
            {

                var soapserializer = SerializerFactory.Get(valuetype);

                taskSource.SetResult(soapserializer.Deserialize(reader));

            }

            return taskSource.Task;
        }
        
        //public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        //{
        //    try
        //    {
        //        var xOver = new XmlAttributeOverrides();
        //        var valuetype = (value as SOAPEnvelope<XmlBodyBase>).body.bodyvalue.GetType();

        //        XmlElementAttribute myElementAttribute = new XmlElementAttribute();
        //        myElementAttribute.ElementName = valuetype.Name;
        //        myElementAttribute.Namespace = "http://ems.whereis.com/";
        //        XmlAttributes myAttributes = new XmlAttributes();
        //        myAttributes.XmlElements.Add(myElementAttribute);

        //        var task = Task.Factory.StartNew(() =>
        //        {

        //            if (valuetype.Name == typeof(geocode).Name)
        //            {
        //                xOver.Add(typeof(MessageBody<geocode>), "bodyvalue", myAttributes);
        //                var soapserializer = new XmlSerializer(typeof(SOAPEnvelope<geocode>), xOver);
        //                soapserializer.Serialize(writeStream, value);
        //            }
        //            else if (valuetype.Name == typeof(geocode).Name)
        //            {

        //            }

        //        });

        //        return task;
        //    }
        //    catch (Exception)
        //    {
        //        return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);
        //    }
        //}
    }
}


