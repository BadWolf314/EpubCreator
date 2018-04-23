using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EpubCreator
{
    public class EpubStructure
    {
        public static string MIMETYPE = "application/epub+zip";
        public static string MIMETYPELOCATION = "mimetype";
        public static string CONTAINER = "<? xml version =\"1.0\"?>\n<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">\n\t<rootfiles>\n\t\t<rootfile full-path=\"CONTENT/package.opf\"\n\t\t\tmedia-type=\"application /oebps-package+xml\" />\n\t</rootfiles>\n</container>";
        public static string METAINFLOCATION = "META-INF";
        public static string CONTAINERLOCATION = METAINFLOCATION + "\\container.xml";
        public static string CONTENTLOCATION = "CONTENT";
        public static string IMAGELOCATION = CONTENTLOCATION + "\\images";
        public static string CSSLOCATION = CONTENTLOCATION + "\\css";
        public static string PACKAGELOCATION = CONTENTLOCATION + "\\package.opf";
        public static string COMMONTITLEPAGE = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                            + "\n<html xmlns = \"http://www.w3.org/1999/xhtml\" xmlns:epub=\"http://www.idpf.org/2007/ops\">"
                                            + "\n<head>"
                                            + "\n<title></title>"
                                            + "<link rel=\"stylesheet\""
                                            + "type=\"text/css\""
                                            + "\nhref=\"css/style.css\" />"
                                            + "<link rel = \"stylesheet\""
                                            + "type=\"text/css\""
                                            + "href=\"css/media.css\" />"
                                            + "</head>"
                                            + "<body>"
                                            + "<section id = \"title-page\" class=\"element titlepage\" epub:type=\"titlepage\">"
                                            + "<div class=\"title-page-title-subtitle-block\">"
                                            + "<h1 class=\"title-page-title\">{0}</h1>"
                                            + "<h3 class=\"title-page-subtitle\">{1}</h3>"
                                            + "</div>"
                                            + "<div class=\"title-page-author-block\">"
                                            + "<h2 class=\"title-page-author title-page-author-1\">{2}</h2>"
                                            + "</div>"
                                            + "</section>"
                                            + "</body>"
                                            + "</html>";
        public static string COMMONPAGE = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                                            + "\n<html xmlns = \"http://www.w3.org/1999/xhtml\" xmlns:epub=\"http://www.idpf.org/2007/ops\">"
                                            + "\n<head>"
                                            + "\n\t{0}"
                                            + "\n</head> "
                                            + "\n<body>"
                                            + "\n\t{1}"
                                            + "\n</body>"
                                            + "</html>";
        public static string COMMONHEADER = "\n<title>{0}</title>"
                                            + "\n<link rel = \"stylesheet\""
                                            + "\n\ttype= \"text/css\""
                                            + "\n\thref= \"css/style.css\" />"
                                            + "\n\t<link rel= \"stylesheet\""
                                            + "\n\ttype= \"text/css\""
                                            + "\n\thref= \"css/media.css\" />";
        public static string COMMONBODY = "\n<section id=\"{0}\" class=\"element chapter\" epub:type=\"chapter\">"
                                            + "\n<header class=\"heading heading-with-title heading-without-image\" id=\"{0}-heading\">"
                                            + "\n\t<div class=\"chapter-title-subtitle-block chapter-title-block-with-chapter-number\">"
                                            + "\n\t<div class=\"title-block\">"
                                            + "\n\t\t<h1 class=\"title\">{1}</h1>"
                                            + "\n\t</div>"
                                            + "\n\t<br />"
                                            + "\n\t</div>"
                                            + "\n</header>"
                                            + "\n<div class=\"text text-main\" id=\"{0}-i-text\">"
                                            + "{2}"
                                            + "\n</div>"
                                            + "\n</section>";
        public static string COMMONPARAGRAPH = "\n<p class=\"subsq\">{0}</p>";
        public static string COMMONSMALLIMAGECONTAINER = "\n<figure class=\"inline-image inline-image-size-small inline-image-flow-center\">{0}\n</figure>";
        public static string COMMONIMAGE = "\n<div class=\"inline-image-container\">"
                                            + "\n<img src=\"{0}\" alt=\"{1}\" />"
                                            + "\n</div>";
        public static string COMMONFULLIMAGEWITHCAPTIONCONTAINER = "\n<figure class=\"inline-image inline-image-kind-photograph inline-image-size-full inline-image-flow-center inline-image-aspect-wide inline-image-with-caption\">"
                                            + "{0}"
                                            + "\n<figcaption class=\"inline-image-caption\">{1}</figcaption>"
                                            + "\n</figure>";
        public static string DEFAULTMEDIACSS = "SupportingFiles\\media.css";
        public static string DEFAULTSTYLECSS = "SupportingFiles\\style.css";

        public static string GetMediaType(string file)
        {
            string mediaType = "application/xhtml+xml";

            if (file.EndsWith(".jpg"))
            {
                mediaType = "image/jpeg";
            }
            else if (file.EndsWith(".png"))
            {
                mediaType = "image/png";
            }
            else if (file.EndsWith(".svg"))
            {
                mediaType = "image/svg+xml";
            }
            else if(file.EndsWith(".css"))
            {
                mediaType = "text/css";
            }

            return mediaType;
        }
    }

    public class Epub
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string author { get; set; }
        public List<Page> pages { get; set; }
        public string location { get; set; }
        public List<string> css { get; set; }
        public Package package { get; set; }
        public Epub() {
            package = new Package();
        }
    }

    public class Page
    {
        public string url { get; set; }
        public string title { get; set; }
        public string parser { get; set; }
        public Page() { }
    }

    [XmlRoot(ElementName = "identifier", Namespace = "http://purl.org/dc/elements/1.1/")]
    public class Identifier
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
    public class Creator
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "meta", Namespace = "http://www.idpf.org/2007/opf")]
    public class Meta
    {
        [XmlAttribute(AttributeName = "refines")]
        public string Refines { get; set; }
        [XmlAttribute(AttributeName = "property")]
        public string Property { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "metadata", Namespace = "http://www.idpf.org/2007/opf")]
    public class Metadata
    {
        [XmlElement(ElementName = "identifier", Namespace = "http://purl.org/dc/elements/1.1/")]
        public Identifier Identifier { get; set; }
        [XmlElement(ElementName = "language", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Language { get; set; }
        [XmlElement(ElementName = "title", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Title { get; set; }
        [XmlElement(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
        public Creator Creator { get; set; }
        [XmlElement(ElementName = "meta", Namespace = "http://www.idpf.org/2007/opf")]
        public List<Meta> Meta { get; set; }
        [XmlAttribute(AttributeName = "dc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dc { get; set; }
    }

    [XmlRoot(ElementName = "item", Namespace = "http://www.idpf.org/2007/opf")]
    public class Item
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "properties")]
        public string Properties { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "media-type")]
        public string Mediatype { get; set; }
    }

    [XmlRoot(ElementName = "manifest", Namespace = "http://www.idpf.org/2007/opf")]
    public class Manifest
    {
        [XmlElement(ElementName = "item", Namespace = "http://www.idpf.org/2007/opf")]
        public List<Item> Item { get; set; }
        public Manifest()
        {
            Item = new List<Item>();
        }
    }

    [XmlRoot(ElementName = "itemref", Namespace = "http://www.idpf.org/2007/opf")]
    public class Itemref
    {
        [XmlAttribute(AttributeName = "idref")]
        public string Idref { get; set; }
    }

    [XmlRoot(ElementName = "spine", Namespace = "http://www.idpf.org/2007/opf")]
    public class Spine
    {
        [XmlElement(ElementName = "itemref", Namespace = "http://www.idpf.org/2007/opf")]
        public List<Itemref> Itemref { get; set; }
        public Spine()
        {
            Itemref = new List<Itemref>();
        }
    }

    [XmlRoot(ElementName = "package", Namespace = "http://www.idpf.org/2007/opf")]
    public class Package
    {
        [XmlElement(ElementName = "metadata", Namespace = "http://www.idpf.org/2007/opf")]
        public Metadata Metadata { get; set; }
        [XmlElement(ElementName = "manifest", Namespace = "http://www.idpf.org/2007/opf")]
        public Manifest Manifest { get; set; }
        [XmlElement(ElementName = "spine", Namespace = "http://www.idpf.org/2007/opf")]
        public Spine Spine { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "unique-identifier")]
        public string Uniqueidentifier { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        public Package()
        {
            Metadata = new Metadata();
            Manifest = new Manifest();
            Spine = new Spine();
        }
    }

}//END Of NAMESPACE
