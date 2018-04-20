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
