using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;
using WhereIsProxy.WhereIsSchemaReference;

namespace WhereIsProxy.Models
{
   
    public class SOAPEnvelopeBase
    {

    }

    [XmlType(Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SOAPEnvelope<T> : SOAPEnvelopeBase
    {
        [XmlAttribute(AttributeName = "soap", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string soapenva { get; set; }

        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public string xsd { get; set; }

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string xsi { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public MessageBody<T> body { get; set; }

        [XmlNamespaceDeclarations] public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
        public SOAPEnvelope()
        {
            xmlns.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");
        }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class MessageBody<T>
    {
        public T bodyvalue { get; set; }
    }
}

    