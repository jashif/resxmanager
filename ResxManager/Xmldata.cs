/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Xml2CSharp
{
    [XmlRoot(ElementName = "import", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Import
    {
        [XmlAttribute(AttributeName = "namespace")]
        public string Namespace { get; set; }
    }

    [XmlRoot(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Element
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "minOccurs")]
        public string MinOccurs { get; set; }
        [XmlElement(ElementName = "complexType", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public ComplexType ComplexType { get; set; }
    }

    [XmlRoot(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Sequence
    {
        [XmlElement(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<Element> Element { get; set; }
    }

    [XmlRoot(ElementName = "attribute", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Attribute
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "use")]
        public string Use { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "ref")]
        public string Ref { get; set; }
        [XmlAttribute(AttributeName = "Ordinal", Namespace = "urn:schemas-microsoft-com:xml-msdata")]
        public string Ordinal { get; set; }
    }

    [XmlRoot(ElementName = "complexType", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class ComplexType
    {
        [XmlElement(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public Sequence Sequence { get; set; }
        [XmlElement(ElementName = "attribute", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<Attribute> Attribute { get; set; }
    }

    [XmlRoot(ElementName = "choice", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Choice
    {
        [XmlElement(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<Element> Element { get; set; }
        [XmlAttribute(AttributeName = "maxOccurs")]
        public string MaxOccurs { get; set; }
    }

    [XmlRoot(ElementName = "schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Schema
    {
        [XmlElement(ElementName = "import", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public Import Import { get; set; }
        [XmlElement(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public Element Element { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "msdata", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Msdata { get; set; }
    }

    [XmlRoot(ElementName = "resheader")]
    public class Resheader
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "data")]
    public class Data
    {
        [XmlElement(ElementName = "aId")]
        public string AId { get; set; }

        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "space", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Space { get; set; }
        [XmlElement(ElementName = "comment")]
        public string Comment { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class Root
    {
        [XmlElement(ElementName = "replace")]
        public string replace { get; set; }
        //[XmlElement(ElementName = "schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
        //public Schema Schema { get; set; }
        //[XmlElement(ElementName = "resheader")]
        //public List<Resheader> Resheader { get; set; }
        [XmlElement(ElementName = "data")]
        public List<Data> Data { get; set; }
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
        public Root()
        {
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            xmlns.Add("dig", "http://www.jio.ril.com/integration/services/fulfilment/DigitalServiceOrderManagement/");
            xmlns.Add("dig1", "http://www.jio.ril.com/information/CanonicalDataModel/DigitalServiceOrderManagement/");
            xmlns.Add("ns0", "http://www.tmforum.org/xml/tip/model");
            xmlns.Add("ns1", "http://www.tmforum.org/xml/tip/model");
            xmlns.Add("ns2", "http://www.tmforum.org/xml/tip/cbe/bi");
        }
    }


    public class XMLUtil
    {
        public static string Serialize(object data)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            StringBuilder sb = new StringBuilder();

            StringWriterWithEncoding stringWriter = new StringWriterWithEncoding(sb, Encoding.UTF8);


            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            XmlWriter writer = XmlWriter.Create(stringWriter, settings);

            var xmlSerializer = new XmlSerializer(data.GetType(), "");

            xmlSerializer.Serialize(writer, data);
            var finalXml = sb.ToString();
            return finalXml;
        }
        public static object DeSerialize(Stream data)
        {

            var xmlSerializer = new XmlSerializer(typeof(Root), "");

            return xmlSerializer.Deserialize(data);

        }
    }
    public class StringWriterWithEncoding : StringWriter
    {
        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            this.m_Encoding = encoding;
        }
        private readonly Encoding m_Encoding;
        public override Encoding Encoding
        {
            get
            {
                return this.m_Encoding;
            }
        }
    }
}
