using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EpubCreator
{
    class EpubStructure
    {
        public static string MIMETYPE = "application/epub+zip";
        public static string MIMETYPELOCATION = "mimetype";
        public static string CONTAINER = "<? xml version =\"1.0\"?>\n<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">\n\t<rootfiles>\n\t\t<rootfile full-path=\"OEBPS /package.opf\"\n\t\t\tmedia-type=\"application /oebps-package+xml\" />\n\t</rootfiles>\n</container>";
        public static string METAINFLOCATION = "META-INF";
        public static string CONTAINERLOCATION = METAINFLOCATION + "\\container.xml";
        public static string OEBPSLOCATION = "OEBPS";
        public static string IMAGELOCATION = OEBPSLOCATION + "\\images";
        public static string CSSLOCATION = OEBPSLOCATION + "\\css";
        public static string PACKAGELOCATION = OEBPSLOCATION + "\\package.opf";
    }

    class Epub
    {
        public string title { get; set; }
        public string author { get; set; }
        public List<Page> pages { get; set; }
        public string location { get; set; }
        public Epub() { }
    }

    class Page
    {
        public string url { get; set; }
        public string title { get; set; }
        public string parser { get; set; }
        public Page() { }
    }

    class Package
    {
        public Metadata metadata { get; set; }
        public Manifest manifest { get; set; }
        public Spine spine { get; set; }
        public Package() { }
    }

    class Metadata
    {

        public Metadata() { }
    }

    class Manifest
    {
        [XmlArray("items")]
        public Item[] items { get; set; }
        public Manifest() { }
    }

    class Spine
    {
        public Spine() { }
    }

    class Item
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string href { get; set; }
        [XmlAttribute("media-type")]
        public string mediatype { get; set; }
    }

}
